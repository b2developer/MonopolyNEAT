using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MONOPOLY
{
    public class Board
    {
        public enum EOutcome
        {
            ONGOING,
            DRAW,
            WIN1,
            WIN2,
            WIN3,
            WIN4,
        }

        public enum EMode
        {
            ROLL,
        }

        public enum ETile
        {
            NONE,
            PROPERTY,
            TRAIN,
            UTILITY,
            CHANCE,
            CHEST,
            TAX,
            JAIL,
        }

        public enum ECard
        {
            ADVANCE,
            RAILROAD2,
            UTILITY10,
            REWARD,
            CARD,
            BACK3,
            JAIL,
            REPAIRS,
            STREET,
            FINE,
            CHAIRMAN,
            BIRTHDAY,
        }

        public class CardEntry
        {
            public ECard card;
            public int val;

            public CardEntry(ECard c, int v)
            {
                card = c;
                val = v;
            }
        }

        //constants
        //--------------------
        public static int PLAYER_COUNT = 4;

        public static int BANK_INDEX = -1;

        public static int BOARD_LENGTH = 40;
        public static int STALEMATE_TURN = 300;

        public static int GO_BONUS = 200;
        public static int GO_LANDING_BONUS = 200;

        public static int JAIL_INDEX = 10;
        public static int JAIL_PENALTY = 50;

        public static float MORTGAGE_INTEREST = 1.1f;

        //penalties for landing on a property (all circumstances)
        public static int[,] PROPERTY_PENALTIES = new int[16, 6]
        {{2, 10, 30, 90, 160, 250 },
        { 4, 20, 60, 180, 320, 450 },
        { 6, 30, 90, 270, 400, 550 },
        { 8, 40, 100, 300, 450, 600 },
        { 10, 50, 150, 450, 625, 750 },
        { 12, 60, 180, 500, 700, 900 },
        { 14, 70, 200, 550, 750, 950 },
        { 16, 80, 220, 600, 800, 1000 },
        { 18, 90, 250, 700, 875, 1050 },
        { 20, 100, 300, 750, 925, 1100 },
        { 22, 110, 330, 800, 975, 1150 },
        { 22, 120, 360, 850, 1025, 1200 },
        { 26, 130, 390, 900, 1100, 1275 },
        { 28, 150, 450, 1000, 1200, 1400 },
        { 35, 175, 500, 1100, 1300, 1500 },
        { 50, 200, 600, 1400, 1700, 2000 }};

        //penalities for landing on utilities (needs to be multiplied by roll)
        public static int[] UTILITY_POSIIONS = new int[2] { 12, 28 };
        public static int[] UTILITY_PENALTIES = new int[2] { 4, 10 };

        //penalties for landing on trains
        public static int[] TRAIN_POSITIONS = new int[4] { 5, 15, 25, 35 };
        public static int[] TRAIN_PENALTIES = new int[4] { 25, 50, 100, 200 };

        public static ETile[] TYPES = new ETile[40] 
        {ETile.NONE, ETile.PROPERTY, ETile.CHEST, ETile.PROPERTY, ETile.TAX, ETile.TRAIN, ETile.PROPERTY, ETile.CHANCE, ETile.PROPERTY, ETile.PROPERTY, ETile.NONE,
         ETile.PROPERTY, ETile.UTILITY, ETile.PROPERTY, ETile.PROPERTY, ETile.TRAIN, ETile.PROPERTY, ETile.CHEST, ETile.PROPERTY, ETile.PROPERTY, ETile.NONE,
         ETile.PROPERTY, ETile.CHANCE, ETile.PROPERTY, ETile.PROPERTY, ETile.TRAIN, ETile.PROPERTY, ETile.PROPERTY, ETile.UTILITY, ETile.PROPERTY, ETile.JAIL,
         ETile.PROPERTY, ETile.PROPERTY, ETile.CHEST, ETile.PROPERTY, ETile.TRAIN, ETile.CHANCE, ETile.PROPERTY, ETile.TAX, ETile.PROPERTY};

        public static int[] COSTS = new int[40] { 0, 60, 0, 60, 200, 200, 100, 0, 100, 120, 0, 140, 150, 140, 160, 200, 180, 0, 180, 200, 0, 220, 0, 220, 240, 200, 260, 260, 150, 280, 0, 300, 300, 0, 320, 200, 0, 250, 100, 400 };
        public static int[] BUILD = new int[16] { 50, 50, 50, 50, 100, 100, 100, 100, 150, 150, 150, 150, 200, 200, 200, 200 };

        public static int[,] SETS = new int[8, 3]
        {{1, 3, -1},
        { 6, 8, 9},
        { 11, 13, 14 },
        { 16, 18, 19 },
        { 21, 23, 24},
        { 26, 27, 29 },
        { 31, 32, 34 },
        { 37, 39, -1 }};
        //--------------------

        //board states
        //--------------------
        public bool[] mortgaged = new bool[40] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };

        public int[] owners = new int[40] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        public int[] property = new int[40] { -1, 0, -1, 1, -1, -1, 2, -1, 2, 3, -1, 4, -1, 4, 5, -1, 6, -1, 6, 7, -1, 8, -1, 8, 9, -1, 10, 10, -1, 11, -1, 12, 12, -1, 13, -1, -1, 14, -1, 15 };

        public int[] houses = new int[40] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        public int[] original = new int[40] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        //--------------------

        public EMode mode = EMode.ROLL;

        public NeuralPlayer[] players;
        public NetworkAdapter adapter;
        public RNG random;

        public int turn = 0;
        public int count = 0;
        public int remaining = 0;

        public int last_roll = 0;

        //card stacks
        //--------------------
        public List<CardEntry> chance;
        public List<CardEntry> chest;
        //--------------------  

        public Board(NetworkAdapter _adapter)
        {
            players = new NeuralPlayer[PLAYER_COUNT];
            random = new RNG();

            adapter = _adapter;

            for (int i = 0; i < PLAYER_COUNT; i++)
            {
                players[i] = new NeuralPlayer();

                adapter.SetPosition(i, players[i].position);
                adapter.SetMoney(i, players[i].funds);           
            }

            remaining = PLAYER_COUNT;

            chance = new List<CardEntry>();
            chest = new List<CardEntry>();

            //chance
            //--------------------
            chance.Add(new CardEntry(ECard.ADVANCE, 39));
            chance.Add(new CardEntry(ECard.ADVANCE, 0));
            chance.Add(new CardEntry(ECard.ADVANCE, 24));
            chance.Add(new CardEntry(ECard.ADVANCE, 11));
            chance.Add(new CardEntry(ECard.RAILROAD2, 0));
            chance.Add(new CardEntry(ECard.RAILROAD2, 0));
            chance.Add(new CardEntry(ECard.UTILITY10, 0));
            chance.Add(new CardEntry(ECard.REWARD, 50));
            chance.Add(new CardEntry(ECard.CARD, 0));
            chance.Add(new CardEntry(ECard.BACK3, 0));
            chance.Add(new CardEntry(ECard.JAIL, 0));
            chance.Add(new CardEntry(ECard.REPAIRS, 0));
            chance.Add(new CardEntry(ECard.FINE, 15));
            chance.Add(new CardEntry(ECard.ADVANCE, 5));
            chance.Add(new CardEntry(ECard.CHAIRMAN, 0));
            chance.Add(new CardEntry(ECard.REWARD, 150));
            chance = random.Shuffle(chance);
            //--------------------

            //chest
            //--------------------
            chest.Add(new CardEntry(ECard.ADVANCE, 0));
            chest.Add(new CardEntry(ECard.REWARD, 200));
            chest.Add(new CardEntry(ECard.FINE, 50));
            chest.Add(new CardEntry(ECard.REWARD, 50));
            chest.Add(new CardEntry(ECard.CARD, 0));
            chest.Add(new CardEntry(ECard.JAIL, 0));
            chest.Add(new CardEntry(ECard.REWARD, 100));
            chest.Add(new CardEntry(ECard.REWARD, 20));
            chest.Add(new CardEntry(ECard.BIRTHDAY, 0));
            chest.Add(new CardEntry(ECard.REWARD, 100));
            chest.Add(new CardEntry(ECard.FINE, 100));
            chest.Add(new CardEntry(ECard.FINE, 50));
            chest.Add(new CardEntry(ECard.FINE, 25));
            chest.Add(new CardEntry(ECard.STREET, 0));
            chest.Add(new CardEntry(ECard.REWARD, 10));
            chest.Add(new CardEntry(ECard.REWARD, 100));
            chest = random.Shuffle(chest);
            //--------------------
        }

        public EOutcome Step()
        {
            switch (mode)
            {
                case EMode.ROLL: return Roll();
            }

            return EOutcome.ONGOING;
        }

        public EOutcome Roll()
        {
            BeforeTurn();

            int d1 = random.gen.Next(1, 7);
            int d2 = random.gen.Next(1, 7);

            last_roll = d1 + d2;

            bool isDouble = d1 == d2;
            bool doubleInJail = false;

            if (players[turn].state == Player.EState.JAIL)
            {
                adapter.SetTurn(turn);
                Player.EJailDecision decision = players[turn].DecideJail();

                if (decision == Player.EJailDecision.ROLL)
                {
                    //regular jail state
                    if (isDouble)
                    {
                        players[turn].jail = 0;
                        players[turn].state = Player.EState.NORMAL;

                        adapter.SetJail(turn, 0);

                        doubleInJail = true;
                    }
                    else
                    {
                        players[turn].jail++;

                        if (players[turn].jail >= 3)
                        {
                            Payment(turn, JAIL_PENALTY);

                            players[turn].jail = 0;
                            players[turn].state = Player.EState.NORMAL;

                            adapter.SetJail(turn, 0);
                        }
                    }
                }
                else if (decision == Player.EJailDecision.PAY)
                {
                    Payment(turn, JAIL_PENALTY);

                    players[turn].jail = 0;
                    players[turn].state = Player.EState.NORMAL;

                    adapter.SetJail(turn, 0);
                }
                else if (decision == Player.EJailDecision.CARD)
                {
                    if (players[turn].card > 0)
                    {
                        players[turn].card--;
                        players[turn].jail = 0;
                        players[turn].state = Player.EState.NORMAL;

                        adapter.SetJail(turn, 0);
                        adapter.SetCard(turn, players[turn].card > 0 ? 1 : 0);
                    }
                    else
                    {
                        //run regular jail state
                        if (isDouble)
                        {
                            players[turn].jail = 0;
                            players[turn].state = Player.EState.NORMAL;

                            adapter.SetJail(turn, 0);
                        }
                        else
                        {
                            players[turn].jail++;

                            if (players[turn].jail >= 3)
                            {
                                Payment(turn, JAIL_PENALTY);

                                players[turn].jail = 0;
                                players[turn].state = Player.EState.NORMAL;

                                adapter.SetJail(turn, 0);
                            }
                        }
                    }
                }
            }

            if (players[turn].state == Player.EState.NORMAL)
            {
                bool notFinalDouble = (!isDouble) || (players[turn].doub <= 1);

                if (notFinalDouble)
                {
                    Movement(d1 + d2, isDouble);
                }
            }

            //start turn again (unless retired or the double was from jail)
            if (players[turn].state != Player.EState.RETIRED && isDouble && !doubleInJail)
            {
                players[turn].doub++;

                if (players[turn].doub >= 3)
                {
                    players[turn].position = JAIL_INDEX;
                    players[turn].doub = 0;
                    players[turn].state = Player.EState.JAIL;

                    adapter.SetJail(turn, 1);
                }
            }

            EOutcome outcome = EndTurn((!isDouble || players[turn].state == Player.EState.RETIRED || players[turn].state == Player.EState.JAIL));
            return outcome;
        }

        public EOutcome EndTurn(bool increment = true)
        {
            if (increment)
            {
                IncrementTurn();

                int count = 0;

                while (players[turn].state == Player.EState.RETIRED && count <= PLAYER_COUNT * 2)
                {
                    IncrementTurn();
                    count++;
                }
                
                if (remaining <= 1)
                {
                    switch (turn)
                    {
                        case 0: return EOutcome.WIN1;
                        case 1: return EOutcome.WIN2;
                        case 2: return EOutcome.WIN3;
                        case 3: return EOutcome.WIN4;
                    }
                }
            }

            count++;

            if (count >= STALEMATE_TURN)
            {
                return EOutcome.DRAW;
            }

            return EOutcome.ONGOING;
        }

        public void IncrementTurn()
        {
            turn++;

            if (turn >= PLAYER_COUNT)
            {
                turn = 0;
            }
        }

        public void BeforeTurn()
        {
            if (players[turn].state == Player.EState.RETIRED)
            {
                return;
            }

            int itemCount = players[turn].items.Count;

            for (int j = 0; j < itemCount; j++)
            {
                int index = players[turn].items[j];

                if (mortgaged[index])
                {
                    int advancePrice = (int)(COSTS[index] * MORTGAGE_INTEREST);

                    if (advancePrice > players[turn].funds)
                    {
                        continue;
                    }

                    adapter.SetTurn(turn);

                    adapter.SetSelectionState(index, 1);

                    Player.EDecision decision = players[turn].DecideAdvance(index);

                    adapter.SetSelectionState(index, 0);

                    if (decision == Player.EDecision.YES)
                    {
                        Advance(index);
                    }
                }
                else
                {
                    adapter.SetTurn(turn);

                    adapter.SetSelectionState(index, 1);

                    Player.EDecision decision = players[turn].DecideMortgage(index);

                    adapter.SetSelectionState(index, 0);

                    if (decision == Player.EDecision.YES)
                    {
                        //Mortgage(index);
                    }
                }
            }

            int[] sets = FindSets(turn);
            int setCount = sets.GetLength(0);

            for (int j = 0; j < setCount; j++)
            {
                int houseTotal = houses[SETS[sets[j], 0]] + houses[SETS[sets[j], 1]];

                if (sets[j] != 0 && sets[j] != 7)
                {
                    houseTotal += houses[SETS[sets[j], 2]];
                }

                int sellMax = houseTotal;

                adapter.SetTurn(turn);

                adapter.SetSelectionState(SETS[sets[j], 0], 1);

                int decision = players[turn].DecideSellHouse(sets[j]);

                adapter.SetSelectionState(SETS[sets[j], 0], 0);

                decision = Math.Min(decision, sellMax);

                if (decision > 0)
                {
                    SellHouses(sets[j], decision);
                    players[turn].funds += (int)(decision * BUILD[property[SETS[sets[j], 0]]] * 0.5f);
                }
            }

            sets = FindSets(turn);
            setCount = sets.GetLength(0);

            for (int j = 0; j < setCount; j++)
            {
                int maxHouse = 10;
                int houseTotal = houses[SETS[sets[j], 0]] + houses[SETS[sets[j], 1]];

                if (sets[j] != 0 && sets[j] != 7)
                {
                    maxHouse = 15;
                    houseTotal += houses[SETS[sets[j], 2]];
                }

                int buildMax = maxHouse - houseTotal;
                int affordMax = (int)Math.Floor(players[turn].funds / (float)BUILD[property[SETS[sets[j], 0]]]);

                if (affordMax < 0)
                {
                    affordMax = 0;
                }

                buildMax = Math.Min(buildMax, affordMax);

                adapter.SetTurn(turn);

                adapter.SetSelectionState(SETS[sets[j], 0], 1);

                int decision = players[turn].DecideBuildHouse(sets[j]);

                adapter.SetSelectionState(SETS[sets[j], 0], 0);

                decision = Math.Min(decision, buildMax);

                if (decision > 0)
                {
                    BuildHouses(sets[j], decision);
                    Payment(turn, decision * BUILD[property[SETS[sets[j], 0]]]);
                }
            }

            Trading();
        }

        public void Trading()
        {
            List<Player> candidates = new List<Player>();
            List<int> candidates_index = new List<int>();

            for (int i = 0; i < PLAYER_COUNT; i++)
            {
                if (i == turn)
                {
                    continue;
                }

                if (players[i].state == Player.EState.RETIRED)
                {
                    continue;
                }

                candidates.Add(players[i]);
                candidates_index.Add(i);
            }

            if (candidates.Count == 0)
            {
                return;
            }

            int TRADE_ATTEMPTS = 4;
            int TRADE_ITEM_MAX = 5;
            int TRADE_MONEY_MAX = 500;

            for (int t = 0; t < TRADE_ATTEMPTS; t++)
            {
                int give = random.gen.Next(0, Math.Min(players[turn].items.Count, TRADE_ITEM_MAX));

                int selectedPlayer = random.gen.Next(0, candidates.Count);

                Player other = candidates[selectedPlayer];
                int other_index = candidates_index[selectedPlayer];

                int recieve = random.gen.Next(0, Math.Min(other.items.Count, TRADE_ITEM_MAX));

                if (players[turn].funds < 0 || other.funds < 0)
                {
                    continue;
                }

                int moneyGive = random.gen.Next(0, Math.Min(players[turn].funds, TRADE_MONEY_MAX));
                int moneyRecieve = random.gen.Next(0, Math.Min(other.funds, TRADE_MONEY_MAX));
                int moneyBalance = moneyGive - moneyRecieve;

                if (give == 0 || recieve == 0)
                {
                    continue;
                }

                List<int> gift = new List<int>();
                List<int> possible = new List<int>(players[turn].items);

                for (int i = 0; i < give; i++)
                {
                    int selection = random.gen.Next(0, possible.Count);

                    gift.Add(possible[selection]);
                    possible.RemoveAt(selection);
                }

                List<int> returning = new List<int>();

                possible = new List<int>(other.items);

                for (int i = 0; i < recieve; i++)
                {
                    int selection = random.gen.Next(0, possible.Count);

                    returning.Add(possible[selection]);
                    possible.RemoveAt(selection);
                }

                //set neurons for trade
                for (int i = 0; i < gift.Count; i++)
                {
                    adapter.SetSelectionState(gift[i], 1);
                }

                for (int i = 0; i < returning.Count; i++)
                {
                    adapter.SetSelectionState(returning[i], 1);
                }

                adapter.SetMoneyContext(moneyBalance);

                Player.EDecision decision = players[turn].DecideOfferTrade();

                if (decision == Player.EDecision.NO)
                {
                    adapter.ClearSelectionState();
                    continue;
                }

                Player.EDecision decision2 = other.DecideAcceptTrade();

                if (decision2 == Player.EDecision.NO)
                {
                    continue;
                }

                for (int i = 0; i < gift.Count; i++)
                {
                    Monopoly.Analytics.instance.MadeTrade(gift[i]);

                    players[turn].items.Remove(gift[i]);
                    other.items.Add(gift[i]);

                    owners[gift[i]] = other_index;
                    adapter.SetOwner(gift[i], other_index);
                }

                for (int i = 0; i < returning.Count; i++)
                {
                    Monopoly.Analytics.instance.MadeTrade(returning[i]);

                    other.items.Remove(returning[i]);
                    players[turn].items.Add(returning[i]);

                    owners[returning[i]] = turn;
                    adapter.SetOwner(returning[i], turn);
                }

                adapter.ClearSelectionState();

                players[turn].funds -= moneyBalance;
                other.funds += moneyBalance;
            }
        }

        public void Auction(int index)
        {
            bool[] participation = new bool[PLAYER_COUNT];

            for (int i = 0; i < PLAYER_COUNT; i++)
            {
                participation[i] = players[i].state != Player.EState.RETIRED;
            }

            int[] bids = new int[PLAYER_COUNT];

            for (int i = 0; i < PLAYER_COUNT; i++)
            {
                adapter.SetTurn(i);

                adapter.SetSelectionState(index, 1);

                bids[i] = players[i].DecideAuctionBid(index);

                adapter.SetSelectionState(index, 0);

                if (bids[i] > players[i].funds)
                {
                    participation[i] = false;
                }
            }

            int max = 0;

            for (int i = 0; i < PLAYER_COUNT; i++)
            {
                if (participation[i])
                {
                    if (bids[i] > max)
                    {
                        max = bids[i];
                    }
                }
            }

            List<int> candidates = new List<int>();
            List<int> backup = new List<int>();

            for (int i = 0; i < PLAYER_COUNT; i++)
            {
                if (participation[i])
                {
                    if (bids[i] == max)
                    {
                        candidates.Add(i);
                    }
                }
                
                if (players[i].state != Player.EState.RETIRED)
                {
                    backup.Add(i);
                }
            }

            if (candidates.Count > 0)
            {
                int winner = candidates[random.gen.Next(0, candidates.Count)];

                Payment(winner, max);

                owners[index] = winner;
                players[winner].items.Add(index);

                if (original[index] == -1)
                {
                    original[index] = winner;
                }

                adapter.SetOwner(index, winner);
            }
            else
            {
                int winner = backup[random.gen.Next(0, backup.Count)];

                owners[index] = winner;
                players[winner].items.Add(index);

                if (original[index] == -1)
                {
                    original[index] = winner;
                }

                adapter.SetOwner(index, winner);
            }
        }

        public void Movement(int roll, bool isDouble)
        {
            players[turn].position += roll;

            //wrap around
            if (players[turn].position >= BOARD_LENGTH)
            {
                players[turn].position -= BOARD_LENGTH;

                if (players[turn].position == 0)
                {
                    players[turn].funds += GO_BONUS;
                }
                else
                {
                    players[turn].funds += GO_LANDING_BONUS;
                }
            }

            adapter.SetMoney(turn, players[turn].funds);
            adapter.SetPosition(turn, players[turn].position);

            ActivateTile();
        }

        public void ActivateTile()
        {
            int index = players[turn].position;

            ETile tile = TYPES[index];

            if (tile == ETile.PROPERTY)
            {
                int owner = Owner(index);

                if (owner == BANK_INDEX)
                {
                    adapter.SetTurn(turn);
                    adapter.SetSelection(index);
                    Player.EBuyDecision decision = players[turn].DecideBuy(index);

                    if (decision == Player.EBuyDecision.BUY)
                    {
                        if (players[turn].funds < COSTS[index])
                        {
                            Auction(index);
                        }
                        else
                        {
                            Payment(turn, COSTS[index]);

                            owners[index] = turn;

                            if (original[index] == -1)
                            {
                                original[index] = turn;
                            }

                            players[turn].items.Add(index);

                            adapter.SetOwner(index, owner);
                        }
                        
                    }
                    else if (decision == Player.EBuyDecision.AUCTION)
                    {
                        Auction(index);
                    }
                }
                else if (owner == turn)
                {
                    //do nothing
                }
                else if (!mortgaged[index])
                {
                    PaymentToPlayer(turn, owner, PROPERTY_PENALTIES[property[index], houses[index]]);
                }
            }
            else if (tile == ETile.TRAIN)
            {
                int owner = Owner(index);

                if (owner == BANK_INDEX)
                {
                    adapter.SetTurn(turn);
                    adapter.SetSelection(index);
                    Player.EBuyDecision decision = players[turn].DecideBuy(index);

                    if (decision == Player.EBuyDecision.BUY)
                    {
                        if (players[turn].funds < COSTS[index])
                        {
                            Auction(index);
                        }
                        else
                        {
                            Payment(turn, COSTS[index]);

                            owners[index] = turn;

                            if (original[index] == -1)
                            {
                                original[index] = turn;
                            }

                            players[turn].items.Add(index);

                            adapter.SetOwner(index, turn);
                        }
                        
                    }
                    else if (owner == turn)
                    {
                        //do nothing
                    }
                    else if (decision == Player.EBuyDecision.AUCTION)
                    {
                        Auction(index);
                    }
                }
                else if (!mortgaged[index])
                {
                    //payment train
                    int trains = CountTrains(owner);

                    if (trains >= 1 && trains <= 4)
                    {
                        int fine = TRAIN_PENALTIES[trains - 1];
                        PaymentToPlayer(turn, owner, fine);
                    }
                    
                }
            }
            else if (tile == ETile.UTILITY)
            {
                int owner = Owner(index);

                if (owner == BANK_INDEX)
                {
                    adapter.SetTurn(turn);

                    adapter.SetSelectionState(index, 1);

                    Player.EBuyDecision decision = players[turn].DecideBuy(index);

                    adapter.SetSelectionState(index, 0);

                    if (decision == Player.EBuyDecision.BUY)
                    {
                        if (players[turn].funds < COSTS[index])
                        {
                            Auction(index);
                        }
                        else
                        {
                            Payment(turn, COSTS[index]);

                            owners[index] = turn;

                            if (original[index] == -1)
                            {
                                original[index] = turn;
                            }

                            players[turn].items.Add(index);

                            adapter.SetOwner(index, turn);
                        }
                            
                    }
                    else if (decision == Player.EBuyDecision.AUCTION)
                    {
                        Auction(index);
                    }
                }
                else if (owner == turn)
                {
                    //do nothing
                }
                else if (!mortgaged[index])
                {
                    //payment utility
                    int utilities = CountUtilities(owner);

                    if (utilities >= 1 && utilities <= 2)
                    {
                        int fine = UTILITY_PENALTIES[utilities - 1] * last_roll;
                        PaymentToPlayer(turn, owner, fine);
                    }    
                }
            }
            else if (tile == ETile.TAX)
            {
                Payment(turn, COSTS[index]);
            }
            else if (tile == ETile.CHANCE)
            {
                DrawChance();
            }
            else if (tile == ETile.CHEST)
            {
                DrawChest();
            }
            else if (tile == ETile.JAIL)
            {
                players[turn].position = JAIL_INDEX;
                players[turn].doub = 0;
                players[turn].state = Player.EState.JAIL;

                adapter.SetJail(turn, 1);
            }
        }

        public void Payment(int owner, int fine)
        {
            players[owner].funds -= fine;
            adapter.SetMoney(owner, players[owner].funds);

            int original = players[owner].funds;

            //prompt for selling sets
            if (players[owner].funds < 0)
            {
                int[] sets = FindSets(turn);
                int setCount = sets.GetLength(0);

                for (int j = 0; j < setCount; j++)
                {
                    int houseTotal = houses[SETS[sets[j], 0]] + houses[SETS[sets[j], 1]];

                    if (sets[j] != 0 && sets[j] != 7)
                    {
                        houseTotal += houses[SETS[sets[j], 2]];
                    }

                    int sellMax = houseTotal;

                    adapter.SetTurn(turn);

                    adapter.SetSelectionState(SETS[sets[j], 0], 1);

                    int decision = players[turn].DecideSellHouse(sets[j]);

                    adapter.SetSelectionState(SETS[sets[j], 0], 0);

                    decision = Math.Min(decision, sellMax);

                    if (decision > 0)
                    {
                        SellHouses(sets[j], decision);

                        players[owner].funds += (int)(decision * BUILD[property[SETS[sets[j], 0]]] * 0.5f);
                        adapter.SetMoney(owner, players[owner].funds);
                    }
                }
            }

            //prompt for mortgages once
            if (players[owner].funds < 0)
            {
                int itemCount = players[owner].items.Count;

                for (int i = 0; i < itemCount; i++)
                {
                    int item = players[owner].items[i];
                    adapter.SetTurn(owner);

                    adapter.SetSelectionState(players[owner].items[i], 1);

                    Player.EDecision decision = players[owner].DecideMortgage(players[owner].items[i]);

                    adapter.SetSelectionState(players[owner].items[i], 0);

                    if (decision == Player.EDecision.YES)
                    {
                        Mortgage(item);
                    }
                }
            }

            //bankrupt
            if (players[owner].funds < 0)
            {
                int regained = players[owner].funds - original;

                int itemCount = players[owner].items.Count;

                int housemoney = 0;

                for (int i = 0; i < itemCount; i++)
                {
                    int item = players[owner].items[i];
                    owners[item] = BANK_INDEX;
                    adapter.SetOwner(item, BANK_INDEX);

                    if (houses[item] > 0)
                    {
                        int liquidated = houses[item];
                        int sell = (liquidated * BUILD[property[item]]) / 2;
                        housemoney += sell;

                        houses[item] = 0;
                    }
                }

                players[owner].items.Clear();

                //give money to other 
                players[owner].state = Player.EState.RETIRED;
                remaining--;
            }
        }

        public void PaymentToPlayer(int owner, int recipient, int fine)
        {
            players[owner].funds -= fine;
            adapter.SetMoney(owner, players[owner].funds);

            players[recipient].funds += fine;
            adapter.SetMoney(recipient, players[recipient].funds);

            int original = players[owner].funds;

            //prompt for selling sets
            if (players[owner].funds < 0)
            {
                int[] sets = FindSets(turn);
                int setCount = sets.GetLength(0);

                for (int j = 0; j < setCount; j++)
                {
                    int houseTotal = houses[SETS[sets[j], 0]] + houses[SETS[sets[j], 1]];

                    if (sets[j] != 0 && sets[j] != 7)
                    {
                        houseTotal += houses[SETS[sets[j], 2]];
                    }

                    int sellMax = houseTotal;

                    adapter.SetTurn(turn);

                    adapter.SetSelectionState(SETS[sets[j], 0], 1);

                    int decision = players[turn].DecideSellHouse(sets[j]);

                    adapter.SetSelectionState(SETS[sets[j], 0], 0);

                    decision = Math.Min(decision, sellMax);

                    if (decision > 0)
                    {
                        SellHouses(sets[j], decision);
                        players[owner].funds += (int)(decision * BUILD[property[SETS[sets[j], 0]]] * 0.5f);

                        adapter.SetMoney(owner, players[owner].funds);
                    }
                }
            }

            //prompt for mortgages once
            if (players[owner].funds < 0)
            {
                int itemCount = players[owner].items.Count;

                for (int i = 0; i < itemCount; i++)
                {
                    int item = players[owner].items[i];
                    adapter.SetTurn(owner);

                    adapter.SetSelectionState(players[owner].items[i], 0);

                    Player.EDecision decision = players[owner].DecideMortgage(players[owner].items[i]);

                    adapter.SetSelectionState(players[owner].items[i], 1);

                    if (decision == Player.EDecision.YES)
                    {
                        Mortgage(item);
                    }
                }
            }

            //bankrupt
            if (players[owner].funds < 0)
            {
                players[recipient].funds += players[owner].funds;
                adapter.SetMoney(recipient, players[recipient].funds);

                int itemCount = players[owner].items.Count;

                int housemoney = 0;

                for (int i = 0; i < itemCount; i++)
                {
                    //give to other player
                    players[recipient].items.Add(players[owner].items[i]);

                    adapter.SetOwner(players[owner].items[i], recipient);

                    int item = players[owner].items[i];
                    owners[item] = recipient;

                    if (houses[item] > 0)
                    {
                        int liquidated = houses[item];
                        int sell = (liquidated * BUILD[property[item]]) / 2;
                        housemoney += sell;

                        houses[item] = 0;
                    }
                }

                players[recipient].funds += housemoney;
                adapter.SetMoney(recipient, players[recipient].funds);

                players[owner].items.Clear();

                //give money to other 
                players[owner].state = Player.EState.RETIRED;
                remaining--;
            }
        }

        public int Owner(int index)
        {
            return owners[index];
        }

        public void Mortgage(int index)
        {
            mortgaged[index] = true;
            adapter.SetMortgage(index, 1);

            players[owners[index]].funds += COSTS[index] / 2;
            adapter.SetMoney(owners[index], players[owners[index]].funds);
        }

        public void Advance(int index)
        {
            mortgaged[index] = false;
            adapter.SetMortgage(index, 0);

            int cost = (int)(COSTS[index] * MORTGAGE_INTEREST);
            Payment(owners[index], cost);
        }

        public int CountTrains(int player)
        {
            int itemCount = players[player].items.Count;

            int count = 0;

            for (int i = 0; i < itemCount; i++)
            {
                if (TRAIN_POSITIONS.Contains(players[player].items[i]))
                {
                    count++;
                }
            }

            return count;
        }

        public int CountUtilities(int player)
        {
            int itemCount = players[player].items.Count;

            int count = 0;

            for (int i = 0; i < itemCount; i++)
            {
                if (UTILITY_POSIIONS.Contains(players[player].items[i]))
                {
                    count++;
                }
            }

            return count;
        }

        public void DrawChance()
        {
            CardEntry card = chance[0];
            chance.RemoveAt(0);
            chance.Add(card);

            if (card.card == ECard.ADVANCE)
            {
                if (players[turn].position > card.val)
                {
                    players[turn].funds += GO_BONUS;
                    adapter.SetMoney(turn, players[turn].funds);
                }

                players[turn].position = card.val;
                adapter.SetPosition(turn, players[turn].position);

                ActivateTile();
            }
            else if (card.card == ECard.REWARD)
            {
                players[turn].funds += card.val;
                adapter.SetMoney(turn, players[turn].funds);
            }
            else if (card.card == ECard.FINE)
            {
                Payment(turn, card.val);
            }
            else if (card.card == ECard.BACK3)
            {
                players[turn].position -= 3;
                adapter.SetPosition(turn, players[turn].position);

                ActivateTile();
            }
            else if (card.card == ECard.CARD)
            {
                players[turn].card++;
                adapter.SetCard(turn, players[turn].card);
            }
            else if (card.card == ECard.JAIL)
            {
                players[turn].position = JAIL_INDEX;
                players[turn].doub = 0;
                players[turn].state = Player.EState.JAIL;

                adapter.SetPosition(turn, players[turn].position);
                adapter.SetJail(turn, 1);
            }
            else if (card.card == ECard.RAILROAD2)
            {
                AdvanceToTrain2();
            }
            else if (card.card == ECard.UTILITY10)
            {
                AdvanceToUtility10();
            }
            else if (card.card == ECard.CHAIRMAN)
            {
                for (int i = 0; i < PLAYER_COUNT; i++)
                {
                    if (i == turn)
                    {
                        continue;
                    }

                    //only pay active players
                    if (players[i].state != Player.EState.RETIRED)
                    {
                        PaymentToPlayer(turn, i, 50);
                    }
                }
            }
            else if (card.card == ECard.REPAIRS)
            {
                int houseCount = 0;
                int hotelCount = 0;
                int itemCount = players[turn].items.Count;

                for (int i = 0; i < itemCount; i++)
                {
                    int index = players[turn].items[i];

                    if (houses[index] <= 4)
                    {
                        houseCount += houses[index];
                    }
                    else
                    {
                        hotelCount++;
                    }    
                }

                Payment(turn, houseCount * 25 + hotelCount * 100);
            }
        }

        public void DrawChest()
        {
            CardEntry card = chest[0];
            chest.RemoveAt(0);
            chest.Add(card);

            if (card.card == ECard.ADVANCE)
            {
                if (players[turn].position > card.val)
                {
                    players[turn].funds += GO_BONUS;
                    adapter.SetMoney(turn, players[turn].funds);
                }

                players[turn].position = card.val;
                adapter.SetPosition(turn, players[turn].position);

                ActivateTile();
            }
            else if (card.card == ECard.REWARD)
            {
                players[turn].funds += card.val;
                adapter.SetMoney(turn, players[turn].funds);
            }
            else if (card.card == ECard.FINE)
            {
                Payment(turn, card.val);
            }
            else if (card.card == ECard.CARD)
            {
                players[turn].card++;
                adapter.SetCard(turn, players[turn].card);
            }
            else if (card.card == ECard.JAIL)
            {
                players[turn].position = JAIL_INDEX;
                players[turn].doub = 0;
                players[turn].state = Player.EState.JAIL;

                adapter.SetPosition(turn, players[turn].position);
                adapter.SetJail(turn, 1);
            }
            else if (card.card == ECard.BIRTHDAY)
            {
                for (int i = 0; i < PLAYER_COUNT; i++)
                {
                    if (i == turn)
                    {
                        continue;
                    }

                    //only pay active players
                    if (players[i].state != Player.EState.RETIRED)
                    {
                        PaymentToPlayer(i, turn, 10);
                    }
                }
            }
            else if (card.card == ECard.STREET)
            {
                int houseCount = 0;
                int hotelCount = 0;
                int itemCount = players[turn].items.Count;

                for (int i = 0; i < itemCount; i++)
                {
                    int index = players[turn].items[i];

                    if (houses[index] <= 4)
                    {
                        houseCount += houses[index];
                    }
                    else
                    {
                        hotelCount++;
                    }
                }

                Payment(turn, houseCount * 40 + hotelCount * 115);
            }
        }

        public void AdvanceToTrain2()
        {
            int index = players[turn].position;

            if (index < TRAIN_POSITIONS[0])
            {
                players[turn].position = TRAIN_POSITIONS[0];
            }
            else if (index < TRAIN_POSITIONS[1])
            {
                players[turn].position = TRAIN_POSITIONS[1];
            }
            else if (index < TRAIN_POSITIONS[2])
            {
                players[turn].position = TRAIN_POSITIONS[2];
            }
            else if (index < TRAIN_POSITIONS[3])
            {
                players[turn].position = TRAIN_POSITIONS[3];
            }
            else
            {
                players[turn].position = TRAIN_POSITIONS[0];
                players[turn].funds += GO_BONUS;
                adapter.SetMoney(turn, players[turn].funds);
            }

            adapter.SetPosition(turn, players[turn].position);

            index = players[turn].position;

            int owner = Owner(index);

            if (owner == BANK_INDEX)
            {
                adapter.SetTurn(turn);

                adapter.SetSelectionState(index, 0);

                Player.EBuyDecision decision = players[turn].DecideBuy(index);

                adapter.SetSelectionState(index, 1);

                if (decision == Player.EBuyDecision.BUY)
                {
                    if (players[turn].funds < COSTS[index])
                    {
                        Auction(index);
                    }
                    else
                    {
                        Payment(turn, COSTS[index]);

                        owners[index] = turn;

                        if (original[index] == -1)
                        {
                            original[index] = turn;
                        }

                        players[turn].items.Add(index);

                        adapter.SetOwner(index, turn);
                    }


                }
                else if (decision == Player.EBuyDecision.AUCTION)
                {
                    Auction(index);
                }
            }
            else if (owner == turn)
            {
                //do nothing
            }
            else if (!mortgaged[index])
            {
                //payment train
                int trains = CountTrains(owner);

                if (trains >= 1 && trains <= 4)
                {
                    int fine = TRAIN_PENALTIES[trains - 1];

                    PaymentToPlayer(turn, owner, fine * 2);
                }
            }
        }

        public void AdvanceToUtility10()
        {
            int index = players[turn].position;

            if (index < UTILITY_POSIIONS[0])
            {
                players[turn].position = UTILITY_POSIIONS[0];
            }
            else if (index < UTILITY_POSIIONS[1])
            {
                players[turn].position = UTILITY_POSIIONS[1];
            }
            else
            {
                players[turn].position = UTILITY_POSIIONS[0];
                players[turn].funds += GO_BONUS;

                adapter.SetMoney(turn, players[turn].funds);
            }

            adapter.SetPosition(turn, players[turn].position);

            index = players[turn].position;

            int owner = Owner(index);

            if (owner == BANK_INDEX)
            {
                adapter.SetTurn(turn);
                Player.EBuyDecision decision = players[turn].DecideBuy(index);

                if (decision == Player.EBuyDecision.BUY)
                {
                    if (players[turn].funds < COSTS[index])
                    {
                        Auction(index);
                    }
                    else
                    {
                        Payment(turn, COSTS[index]);

                        owners[index] = turn;

                        if (original[index] == -1)
                        {
                            original[index] = turn;
                        }

                        players[turn].items.Add(index);

                        adapter.SetOwner(index, turn);
                    }   
                }
                else if (decision == Player.EBuyDecision.AUCTION)
                {
                    Auction(index);
                }
            }
            else if (owner == turn)
            {
                //do nothing
            }
            else if (!mortgaged[index])
            {
                //payment utility
                int fine = 10 * last_roll;

                PaymentToPlayer(turn, owner, fine);
            }
        }

        public int[] FindSets(int owner)
        {
            List<int> sets = new List<int>();
            List<int> items = players[owner].items;

            for (int i = 0; i < 8; i++)
            {
                //two piece sets
                if (i == 0 || i == 7)
                {
                    if (items.Contains(SETS[i,0]) && items.Contains(SETS[i, 1]))
                    {
                        sets.Add(i);
                    }

                    continue;
                }

                //three piece sets
                if (items.Contains(SETS[i, 0]) && items.Contains(SETS[i, 1]) && items.Contains(SETS[i, 2]))
                {
                    sets.Add(i);
                }
            }

            return sets.ToArray();
        }

        public void BuildHouses(int set, int amount)
        {
            int last = 2;

            if (set == 0 || set == 7)
            {
                last = 1;
            }

            for (int i = 0; i < amount; i++)
            {
                //find smallest house number from back
                int bj = last;

                for (int j = last - 1; j >= 0; j--)
                {
                    if (houses[SETS[set, bj]] > houses[SETS[set, j]])
                    {
                        bj = j;
                    }
                }

                houses[SETS[set, bj]]++;
                adapter.SetHouse(SETS[set, bj], houses[SETS[set, bj]]);
            }
        }

        public void SellHouses(int set, int amount)
        {
            int last = 2;

            if (set == 0 || set == 7)
            {
                last = 1;
            }

            for (int i = 0; i < amount; i++)
            {
                //find smallest house number from back
                int bj = 0;

                for (int j = 0; j <= last; j++)
                {
                    if (houses[SETS[set, bj]] < houses[SETS[set, j]])
                    {
                        bj = j;
                    }
                }

                houses[SETS[set, bj]]--;
                adapter.SetHouse(SETS[set, bj], houses[SETS[set, bj]]);
            }
        }
    }
}
