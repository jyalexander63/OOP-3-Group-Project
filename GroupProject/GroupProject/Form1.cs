using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private Player GetCurrentPlayer()
        {
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
            int desiredWidth = 65;
            int desiredHeight = 90;

            foreach (Card card in playerHand)
            {
                Button cardButton = new Button();
                cardButton.Text = $"{card.Value} of {card.Suit}";
                cardButton.Size = new Size(34, 56);


                int imageIndex = GetImageIndexForCard(card);
                if (imageIndex != -1)
                {
                    Image originalImage = imageList1.Images[imageIndex];
                    Image resizedImage = new Bitmap(originalImage, desiredHeight, desiredWidth);

                    imageList1.Images.Add(resizedImage);

                    cardButton.BackgroundImage = imageList1.Images[imageIndex];
                    cardButton.BackgroundImageLayout = ImageLayout.Zoom;
                }
                cardButton.Click += CardButton_Click;
                cardButton.Tag = card;

                Console.WriteLine($"Button for {card.Value} of {card.Suit} created");

                flowLayoutPanel.Controls.Add(cardButton);
            }
        }



        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            InitializePlayers();
            InitializeDeck();

            firstDealer = DetermineFirstDealer();
            MessageBox.Show(firstDealer.Name + " is the first dealer.");

            int dealerIndex = players.IndexOf(firstDealer);
            currentPlayerIndex = (dealerIndex + 1) % players.Count;
            leadPlayer = players[currentPlayerIndex];

            MessageBox.Show($"{leadPlayer.Name} starts the game");
            DealCardsToPlayers();
            
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

            // Deal cards to all FlowLayoutPanel controls
            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            {
                List<Card> hand = players[playerIndex].Hand;
                FlowLayoutPanel flowLayoutPanel = GetPlayerFlowLayout(playerIndex);
                Console.WriteLine($"Layout panel for player {playerIndex + 1}, found: {flowLayoutPanel != null}");
                //string flowLayoutPanelName = "flowLayoutPanel" + (playerIndex + 1);
                //FlowLayoutPanel flowLayoutPanel = (FlowLayoutPanel)this.Controls.Find(flowLayoutPanelName, true).FirstOrDefault();

                if (flowLayoutPanel != null)
                {
                    PopulatePlayersHand(hand, flowLayoutPanel);
                }
                else
                {
                    MessageBox.Show($"FlowLayoutPanel '{"flowLayoutPanel" + (playerIndex +1)}' not found,");
                }
            }
        }

        private void RefreshPlayerHandDisplay(int playerIndex)
        {
            FlowLayoutPanel flowLayoutPanel = GetPlayerFlowLayout(playerIndex);
            Console.WriteLine($"Retrieving layout panel for Player {playerIndex + 1}, found: {flowLayoutPanel != null}");  // Debug output

            if (flowLayoutPanel != null)
            {
                flowLayoutPanel.Controls.Clear();  // Clear the current card display
                PopulatePlayersHand(players[playerIndex].Hand, flowLayoutPanel);  // Repopulate it
            }
            else
            {
                MessageBox.Show("Error: Unable to find FlowLayoutPanel for player " + (playerIndex + 1));
            }
        }


        private FlowLayoutPanel GetPlayerFlowLayout(int playerIndex)
        {
            // Construct the name of the FlowLayoutPanel based on the player index
            string flowLayoutPanelName = "flowLayoutPanel" + (playerIndex + 1).ToString();
            // Find the FlowLayoutPanel by name within the Form's Controls
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



            switch (card.Suit)
            {
                case Suit.Hearts:
                    imageIndex = (int)card.Value - 1;
                    break;
                case Suit.Diamonds:
                    imageIndex = 13 + (int)card.Value - 1;
                    break;
                case Suit.Clubs:
                    imageIndex = 26 + (int)card.Value - 1;
                    break;
                case Suit.Spades:
                    imageIndex = 39 + (int)card.Value - 1;
                    break;
                default:

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


            // Shuffle the deck after dealing cards
            dealerDeck.Shuffle();

            return firstDealer;
        }


        //private void PlayGame()
        //{
        //    // Start a loop representing each round of the game
        //    for (int round = 0; round < 13; round++)
        //    {
        //        // Reset the heartsPlayed flag for each round
        //        heartsPlayed = false;

        //        // Determine the lead player for each round
        //        Player leadPlayer = firstDealer; // Start with the first dealer

        //        foreach (Player currentPlayer in players)
        //        {
        //            // Determine the valid cards the current player can play
        //            List<Card> validCards = GetValidCardsToPlay(currentPlayer, leadPlayer);

        //            // Allow the player to play a card
        //            Card playedCard = currentPlayer.PlayCard(validCards);

        //            // Update the game state or UI with the played card

        //            // Check if hearts have been played
        //            if (!heartsPlayed && playedCard.Suit == Suit.Hearts)
        //            {
        //                heartsPlayed = true;
        //            }


        //        }

        //        // Determine the winner of the trick
        //        Player trickWinner = DetermineTrickWinner(leadPlayer, GetCardsPlayedInTrick(players));

        //        // Update first dealer for the next round
        //        firstDealer = trickWinner;

        //        // Handle the end of the round, score counting, etc.
        //        CalculateScores();
        //        UpdateScoresDisplay();


        //    }
        //    // Game loop ends when all rounds are completed
        //    CalculateScores();
        //    UpdateScoresDisplay();
        //    PopulatePlayersHand(GetCurrentPlayer().Hand, flowLayoutPanel4);
        //}

        private List<Card> GetValidCardsToPlay(Player player, Player leadPlayer)
        {
            Console.WriteLine("Called GetValidCardsToPlay");
            Console.WriteLine($"Current Player: {player.Name}, Lead Player: {leadPlayer?.Name}");

            List<Card> validCards = new List<Card>();
            Card leadCard = (leadPlayer != player) ? leadPlayer?.LastPlayedCard : null;  // Check if lead player has played a card

            if (player == leadPlayer)  // If the current player is the lead player
            {
                if (!heartsPlayed)  // Hearts have not been broken
                {
                    validCards = player.Hand.Where(card => card.Suit != Suit.Hearts).ToList();
                    if (validCards.Count == 0)  // Only allow hearts if no other suits are available
                    {
                        validCards = new List<Card>(player.Hand);  // Allow all cards if only hearts are in hand
                    }
                }
                else
                {
                    validCards = new List<Card>(player.Hand);  // Allow any card if hearts are broken
                }
            }
            else  // Logic for following players
            {
                if (leadCard != null)  // There is a lead card to follow
                {
                    validCards = player.Hand.Where(card => card.Suit == leadCard.Suit).ToList();
                    if (validCards.Count == 0)  // If the player cannot follow suit
                    {
                        validCards = new List<Card>(player.Hand);  // They can play any card
                    }
                }
                else  // This block is theoretically not needed as lead player should always play first
                {
                    validCards = new List<Card>(player.Hand);
                }
            }

            Console.WriteLine("Valid cards this turn:");
            foreach (var card in validCards)
            {
                Console.WriteLine($"{card.Value} of {card.Suit}");
            }

            return validCards;
        }

        private void UpdateGameAfterCardPlayed(Player currentPlayer, Card playedCard)
        {
            // Mark hearts as played if a heart card is played and they haven't been broken yet
            if (playedCard.Suit == Suit.Hearts && !heartsPlayed)
            {
                heartsPlayed = true;
                Console.WriteLine("Hearts have now been broken.");
            }

            // Check if all players have played to move to the next round
            if (AllPlayersHavePlayed())
            {
                Player trickWinner = DetermineTrickWinner(leadPlayer, GetCardsPlayedInTrick(players));
                leadPlayer = trickWinner;  // Update the lead player for the next round
                currentPlayerIndex = players.IndexOf(trickWinner);  // Update current player to the trick winner
            }
            else
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;  // Move to the next player
            }

            UpdateScoresDisplay();  // Refresh UI with the latest scores
        }

        private bool AllPlayersHavePlayed()
        {
            // Check if all players have played their cards for the current round
            return players.All(p => p.LastPlayedCard != null);
        }


        private Player DetermineTrickWinner(Player leadPlayer, List<Card> cardsPlayed)
        {
            Suit leadSuit = cardsPlayed.First().Suit; // Get the lead suit of the trick
            Card highestCard = cardsPlayed.First(); // Assume the first card played is the highest initially
            Player trickWinner = leadPlayer; // Assume the lead player is the winner initially

            // Loop through the cards played in the trick
            foreach (var card in cardsPlayed.Skip(1)) // Skip the first card since it's already assumed to be the highest
            {
                // If the current card is of the lead suit and has a higher value than the current highest card
                if (card.Suit == leadSuit && card.Value > highestCard.Value)
                {
                    highestCard = card;
                    trickWinner = players.FirstOrDefault(player => player.Hand.Contains(card));
                }
            }

            return trickWinner;
        }

        private List<Card> GetCardsPlayedInTrick(List<Player> players)
        {
            List<Card> cardsPlayed = new List<Card>();

            foreach (var player in players)
            {
                // Assuming each player keeps track of the last played card
                Card lastPlayedCard = player.LastPlayedCard;
                if (lastPlayedCard != null)
                {
                    cardsPlayed.Add(lastPlayedCard);
                }
            }

            return cardsPlayed;
        }

        private void CalculateScores()
        {
            foreach (Player player in players)
            {
                int score = 0;

                // Count the number of hearts and check for the Queen of Spades
                int heartCount = player.Hand.Count(card => card.Suit == Suit.Hearts);
                bool hasQueenOfSpades = player.Hand.Any(card => card.Suit == Suit.Spades && card.Value == 12);

                // Assign points for hearts and the Queen of Spades
                score += heartCount;
                if (hasQueenOfSpades)
                {
                    score += 13; // Queen of Spades is worth 13 points
                }

                // Update the player's score
                player.Score = score;
            }

            // Now you can display or use the scores as needed
        }

        private void CheckEndGameCondition(int scoreLimit)
        {
            foreach (var player in players)
            {
                if (player.Score >= scoreLimit)
                {
                    MessageBox.Show(player.Name + " has won the game with a score of " + player.Score + "!");
                    // You can add any additional logic here for ending the game or resetting the scores
                    return;
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CardButton_Click(object sender, EventArgs e)
        {
            
            Button clickedButton = (Button)sender;
            Card selectedCard = (Card)clickedButton.Tag;
            Player currentPlayer = GetCurrentPlayer();

            List<Card> validCards = GetValidCardsToPlay(currentPlayer, leadPlayer);
            Console.WriteLine($"Trying to play {selectedCard.Value} of {selectedCard.Suit}");
           

            if (validCards.Contains(selectedCard))
            {
                Console.WriteLine("Card played successfully.");
                currentPlayer.PlayCard(selectedCard); // This assumes PlayCard just removes the card from the hand
                UpdateGameAfterCardPlayed(currentPlayer, selectedCard); // This will handle game logic after a card is played
                RefreshPlayerHandDisplay(players.IndexOf(currentPlayer));
                AddCardToListView(selectedCard);
            }
            else
            {
                MessageBox.Show("Invalid Card Selected");
            }
            
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void flowLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void InitializeListView()
        {
            listView1.View = View.Details;
            listView1.Columns.Add("Value", 70);
            listView1.Columns.Add("Suit", 70);
            listView1.FullRowSelect = true;  // Optional, ensures full row is selectable
        }

        private void AddCardToListView(Card card)
        {
            ListViewItem item = new ListViewItem(card.Value.ToString());
            item.SubItems.Add(card.Suit.ToString());
            listView1.Items.Add(item); // Ensure this is the correct ListView name
        }
    }
}
