using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GroupProject
{
    public partial class Form1 : Form
    {
        private List<Player> players;
        private Deck deck;
        private bool heartsPlayed;
        private Player firstDealer;
        private int currentPlayerIndex;
        private Player leadPlayer;
        private int roundCounter = 0;
        public Form1()
        {
            InitializeComponent();
            InitializeListView();
            ToolTip tooltip1 = new ToolTip();
            toolTip1.SetToolTip(btnStart, "Start the game");

            ToolTip tooltip2 = new ToolTip();
            toolTip2.SetToolTip(btnQuit, "Close application");

            ToolTip tooltip3 = new ToolTip();
            toolTip3.SetToolTip(btnView, "Reveal your cards");
        }

        private void Log(string message)
        {
            string logFilePath = "gameLog.txt";
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(message);
            }
        }

        private Player GetCurrentPlayer()
        {
            if (currentPlayerIndex < 0 || currentPlayerIndex >= players.Count)
            {
                Console.WriteLine($"Error: currentPlayerIndex out of range. Index: {currentPlayerIndex}, Players count: {players.Count}");
                throw new ArgumentOutOfRangeException(nameof(currentPlayerIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }
            return players[currentPlayerIndex];
        }
        private void UpdateScoresDisplay()
        {
            lblPlayer1Score.Text = $"Score: {players[0].Score}";
            lblPlayer2Score.Text = $"Score: {players[1].Score}";
            lblPlayer3Score.Text = $"Score: {players[2].Score}";
            lblPlayer4Score.Text = $"Score: {players[3].Score}";
        }
        private void PopulatePlayersHand(List<Card> playerHand, FlowLayoutPanel flowLayoutPanel)
        {
            int desiredWidth = 60;
            int desiredHeight = 90;

            foreach (Card card in playerHand)
            {
                Button cardButton = new Button();
                //cardButton.Text = $"{card.FaceValue} of {card.Suit}";
                cardButton.Text = $"";
                cardButton.Size = new Size(60, 90);

                int imageIndex = GetImageIndexForCard(card);
                if (imageIndex != -1)
                {
                    Image originalImage = imageList1.Images[imageIndex];
                    Bitmap resizedImage = new Bitmap(desiredWidth, desiredHeight);

                    using (Graphics g = Graphics.FromImage(resizedImage))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(originalImage, 0, 0, desiredWidth, desiredHeight);
                    }

                    imageList1.Images.Add(resizedImage);

                    cardButton.BackgroundImage = imageList1.Images[imageIndex];
                    cardButton.BackgroundImageLayout = ImageLayout.Zoom;
                }
                cardButton.Click += CardButton_Click;
                cardButton.Tag = card;

                //Console.WriteLine($"Button for {card.Value} of {card.Suit} created");

                flowLayoutPanel.Controls.Add(cardButton);
            }
        }
        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Log($"\nGame started at {DateTime.Now}\n");
            InitializePlayers();
            InitializeDeck();

            firstDealer = DetermineFirstDealer();
            MessageBox.Show(firstDealer.Name + " is the first dealer.");

            int dealerIndex = players.IndexOf(firstDealer);
            currentPlayerIndex = (dealerIndex + 1) % players.Count;
            leadPlayer = players[currentPlayerIndex];

            MessageBox.Show($"{leadPlayer.Name} starts the game");
            DealCardsToPlayers();

            // Log the start of a new round
            Log($"\n--- Round {roundCounter + 1} ---\n");
        }
        private void InitializePlayers()
        {
            players = new List<Player>
            {
                new Player("Player 1"),
                new Player("Player 2"),
                new Player("Player 3"),
                new Player("Player 4")
            };
        }
        private void InitializeDeck()
        {
            deck = new Deck();
            deck.Shuffle();
        }
        private void DealCardsToPlayers()
        {
            foreach (Player player in players)
            {
                for (int i = 0; i < 13; i++)
                {
                    Card card = deck.DealCard();
                    player.AddCardToHand(card);
                }
            }

            foreach (Player player in players)
            {
                Log($"{player.Name} was dealt: {string.Join(",", player.Hand.Select(card => $"{card.FaceValue} of {card.Suit}"))}");
            }

            // Deal cards to all FlowLayoutPanel controls
            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            {
                List<Card> hand = players[playerIndex].Hand;
                FlowLayoutPanel flowLayoutPanel = GetPlayerFlowLayout(playerIndex);
                


                if (flowLayoutPanel != null)
                {
                    PopulatePlayersHand(hand, flowLayoutPanel);
                }
                else
                {
                    MessageBox.Show($"FlowLayoutPanel '{"flowLayoutPanel" + (playerIndex + 1)}' not found,");
                }
            }
        }
        private void RefreshPlayerHandDisplay(int playerIndex)
        {
            FlowLayoutPanel flowLayoutPanel = GetPlayerFlowLayout(playerIndex);
             

            if (flowLayoutPanel != null)
            {
                flowLayoutPanel.Controls.Clear();
                PopulatePlayersHand(players[playerIndex].Hand, flowLayoutPanel);
            }
            else
            {
                MessageBox.Show("Error: Unable to find FlowLayoutPanel for player " + (playerIndex + 1));
            }
        }
        private FlowLayoutPanel GetPlayerFlowLayout(int playerIndex)
        {

            string flowLayoutPanelName = "flowLayoutPanel" + (playerIndex + 1).ToString();

            FlowLayoutPanel flowLayoutPanel = this.Controls.Find(flowLayoutPanelName, true).FirstOrDefault() as FlowLayoutPanel;

            if (flowLayoutPanel == null)
            {
                MessageBox.Show($"FlowLayoutPanel '{flowLayoutPanelName}' not found. Check if it's correctly named and initialized.");
            }

            return flowLayoutPanel;
        }
        private int GetImageIndexForCard(Card card)
        {
            int imageIndex = -1;

            // Calculate the base index based on the card's suit
            switch (card.Suit)
            {
                case Suit.Clubs:
                    imageIndex = 0;
                    break;
                case Suit.Diamonds:
                    imageIndex = 13; // Offset of 13 for diamonds
                    break;
                case Suit.Hearts:
                    imageIndex = 26; // Offset of 26 for hearts
                    break;
                case Suit.Spades:
                    imageIndex = 39; // Offset of 39 for spades
                    break;
                default:
                    break;
            }

            // Add an offset based on the card's value for Ace, Jack, King, and Queen
            switch (card.Value)
            {
                case 14: // Ace
                    imageIndex += 12;
                    break;
                case 11: // Jack
                    imageIndex += 9;
                    break;
                case 13: // King
                    imageIndex += 11;
                    break;
                case 12: // Queen
                    imageIndex += 10;
                    break;
                default: // 2 through 10
                    imageIndex += card.Value - 2;
                    break;
            }
            return imageIndex;
        }

        private Player DetermineFirstDealer()
        {
            Deck dealerDeck = new Deck();
            dealerDeck.Shuffle();
            Player firstDealer = null;
            int highestCardValue = -1;
            List<Card> drawnCards = new List<Card>();
            foreach (Player player in players)
            {
                // Deal a card from the deck
                Card drawnCard = dealerDeck.DealCard();
                drawnCards.Add(drawnCard);

                // Update highest card and first dealer
                if (drawnCard.Value > highestCardValue)
                {
                    highestCardValue = drawnCard.Value;
                    firstDealer = player;
                }
            }
            foreach (Card card in drawnCards)
            {
                dealerDeck.Cards.Add(card);
            }
            dealerDeck.Shuffle();
            return firstDealer;
            
        }
        private List<Card> GetValidCardsToPlay(Player player, Player leadPlayer)
        {
            Console.WriteLine("Called GetValidCardsToPlay");
            Console.WriteLine($"Current Player: {player.Name}, Lead Player: {leadPlayer?.Name}");

            List<Card> validCards = new List<Card>();
            Card leadCard = (leadPlayer != player) ? leadPlayer?.LastPlayedCard : null;

            if (player == leadPlayer)
            {
                if (!heartsPlayed)
                {
                    validCards = player.Hand.Where(card => card.Suit != Suit.Hearts).ToList();
                    if (validCards.Count == 0)
                    {
                        validCards = new List<Card>(player.Hand);
                    }
                }
                else
                {
                    validCards = new List<Card>(player.Hand);
                }
            }
            else
            {
                if (leadCard != null)
                {
                    validCards = player.Hand.Where(card => card.Suit == leadCard.Suit).ToList();
                    if (validCards.Count == 0)
                    {
                        validCards = new List<Card>(player.Hand);
                    }
                }
                else
                {
                    validCards = new List<Card>(player.Hand);
                }
            }

            Console.WriteLine("Valid cards this turn:");
            foreach (var card in validCards)
            {
                Console.WriteLine($"{card.FaceValue} of {card.Suit}");
            }

            return validCards;
        }
        private bool AllPlayersHavePlayed()
        {
            return players.All(p => p.LastPlayedCard != null);
        }
        private void UpdateGameAfterCardPlayed(Player currentPlayer, Card playedCard)
        {
            
            if (AllPlayersHavePlayed())
            {
                // A dictionary to keep track of who played which card
                Dictionary<Card, Player> cardPlayerDict = new Dictionary<Card, Player>();

                foreach (Player player in players)
                {
                    if (player.LastPlayedCard != null)
                    {
                        cardPlayerDict[player.LastPlayedCard] = player;
                    }
                }
                Player trickWinner = DetermineTrickWinner(leadPlayer, GetCardsPlayedInTrick(players), cardPlayerDict);
                Console.WriteLine($"Trick winner: {trickWinner.Name}");
               
                leadPlayer = trickWinner;
                currentPlayerIndex = players.IndexOf(trickWinner);

                // Add the played cards to the trick winner's pile of cards
                foreach (Card card in GetCardsPlayedInTrick(players))
                {
                    trickWinner.TakenCards.Add(card);
                }

                // Reset the LastPlayedCard of each player
                foreach (Player player in players)
                {
                    player.ResetLastPlayedCard();
                }


                

                roundCounter++;

                // Log the start of a new round
                Log($"\n--- Round {roundCounter + 1} ---\n");
                
                if (roundCounter == 13)
                {
                    CalculateScores();

                    lblPlayer1Score.Text = $"Score: {players[0].Score}";
                    lblPlayer2Score.Text = $"Score: {players[1].Score}";
                    lblPlayer3Score.Text = $"Score: {players[2].Score}";
                    lblPlayer4Score.Text = $"Score: {players[3].Score}";

                    Player winner = players.OrderBy(player => player.Score).First();
                    MessageBox.Show($"{winner.Name} Wins! with a score of {winner.Score}!");
                }
            }
            else
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            }

            UpdateScoresDisplay();
        }
        private void CalculateScores()
        {
            foreach (Player player in players)
            {
                int score = 0;
                foreach (Card card in player.TakenCards)
                {
                    if (card.Suit == Suit.Hearts)
                    {
                        score += 1;
                    }
                    else if (card.Suit == Suit.Spades && card.Value == 12) 
                    {
                        score += 5;
                    }
                }
                player.Score = score;
            }
            lblPlayer1Score.Text = $"Score: {players[0].Score}";
            lblPlayer2Score.Text = $"Score: {players[1].Score}";
            lblPlayer3Score.Text = $"Score: {players[2].Score}";
            lblPlayer4Score.Text = $"Score: {players[3].Score}";
        }
        private Player DetermineTrickWinner(Player leadPlayer, List<Card> cardsPlayed, Dictionary<Card, Player> cardPlayerDict)
        {
            // Initialize the highest card as the lead players last played card
            Card highestCard = leadPlayer.LastPlayedCard;
            // set the lead player as the winner if anything fails
            Player trickWinner = leadPlayer; 

            foreach (var card in cardsPlayed)
            {
                // Find the player who played this card
                Player playerWhoPlayed = cardPlayerDict[card];
                if (playerWhoPlayed == null)
                {
                    Console.WriteLine($"Error: Card {card.FaceValue} of {card.Suit} is not found in any player's hand.");
                    continue; 
                }

                // Check if this card is of the lead suit and has a higher value than the current highest card
                if (card.Suit == highestCard.Suit && card.Value > highestCard.Value)
                {
                    highestCard = card;
                    trickWinner = playerWhoPlayed; 
                    Console.WriteLine($"New highest card found: {card.FaceValue} of {card.Suit} played by {playerWhoPlayed.Name}.");
                }
            }

            if (trickWinner == null)
            {
                Console.WriteLine("Critical Error: No trick winner determined. Defaulting to lead player.");
                return leadPlayer;
            }
            return trickWinner;
        }
        private List<Card> GetCardsPlayedInTrick(List<Player> players)
        {
            List<Card> cardsPlayed = new List<Card>();

            foreach (var player in players)
            {
                
                Card lastPlayedCard = player.LastPlayedCard;
                if (lastPlayedCard != null)
                {
                    cardsPlayed.Add(lastPlayedCard);
                }
            }

            return cardsPlayed;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void CardButton_Click(object sender, EventArgs e)
        {
            
            Button clickedButton = (Button)sender;
            Card selectedCard = (Card)clickedButton.Tag;
            Player currentPlayer = GetCurrentPlayer();
            Console.WriteLine($"Current player index: {currentPlayerIndex}");

            List<Card> validCards = GetValidCardsToPlay(currentPlayer, leadPlayer);
            Console.WriteLine($"Trying to play {selectedCard.FaceValue} of {selectedCard.Suit}");

            if (validCards.Contains(selectedCard))
            {

                Console.WriteLine("Card played successfully.");
                Log($"{currentPlayer.Name} played: {selectedCard.FaceValue} of {selectedCard.Suit}");
                currentPlayer.PlayCard(selectedCard);
                UpdateGameAfterCardPlayed(currentPlayer, selectedCard);
                RefreshPlayerHandDisplay(players.IndexOf(currentPlayer));
                AddCardToListView(selectedCard);

                
            }
            else
            {
                MessageBox.Show("Invalid Card Selected");
            }
            
        }
        private void InitializeListView()
        {
            listView1.View = View.Details;
            listView1.Columns.Add("Value", 70);
            listView1.Columns.Add("Suit", 70);
            listView1.FullRowSelect = true;
        }
        private void AddCardToListView(Card card)
        {
            ListViewItem item = new ListViewItem(card.FaceValue.ToString());
            item.SubItems.Add(card.Suit.ToString());
            listView1.Items.Add(item);
        }
    }
}