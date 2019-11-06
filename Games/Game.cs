using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Timers;
using System.Diagnostics;

namespace Panic
{
    public abstract class Game<TPlayer> : INotifyPropertyChanged
        where TPlayer : Player
    {
        public FixedSprite[] countdownNumbers;

        public Game()
        {
            doRestartGame();
            countdownNumbers = new FixedSprite[] 
            { 
                new FixedSprite(Properties.Resources.picturetopeople_org_0cf714287edb63347deadeb1e3837903751eee8a5dc2a163c2.ToFrame(), new[] {0d, 0d}, new[] {0d,0d}),
                new FixedSprite(Properties.Resources.picturetopeople_org_bccd4c9d1aa31d837572a2198667964f12e060ba01f8b92bd5.ToFrame(), new[] {0d, 0d}, new[] {0d,0d}),
                new FixedSprite(Properties.Resources.picturetopeople_org_87b38ae0fb2ab421460c1fdbebb1fdfe4405f36ebf905e8f55.ToFrame(), new[] {0d, 0d}, new[] {0d,0d}),
                new FixedSprite(Properties.Resources.picturetopeople_org_16c24f4d0d65ad5a3ce6a81c2c7d3cf64478694fe4516c6271.ToFrame(), new[] {0d, 0d}, new[] {0d,0d})
            };

            foreach (FixedSprite s in countdownNumbers)
                s.centreOnCanvas(width, height);

            playerSelectionVM = new PlayerSelectionVM(puterScripts.Count(), Properties.Resources.black_vehicles);

            players = new ObservableCollection<TPlayer>();
            players.CollectionChanged += (s, e) =>
                {
                    RaisePropertyChanged("players");
                    RaisePropertyChanged("humanPlayers");
                    RaisePropertyChanged("puterPlayers");
                };
        }

        private TPlayer _selectedPlayerForEdit;
        public TPlayer selectedPlayerForEdit 
        {
            get { return _selectedPlayerForEdit; }
            set
            {
                if (_selectedPlayerForEdit != value)
                {
                    _selectedPlayerForEdit = value;
                    RaisePropertyChanged("selectedPlayerForEdit");
                }
            }
        }

        public abstract string defaultPythonCode { get; }
        public abstract string HTMLInstructions { get; }

        public abstract int width { get; }
        public abstract int height { get; }
        public abstract string scoreNames { get; }

        public ObservableCollection<FixedSprite > _sprites;
        protected Stack<Key[]> keyActions;
        public Key[] keysDownMomentary { get; private set; }
        public Key[] keysToCheckPermanent { get; protected set; }

        /// <summary>
        /// returns null if no keys are available
        /// </summary>
        /// <param name="playerSelectionVM"></param>
        /// <returns></returns>
        public abstract TPlayer createPlayerFromSelection(PlayerSelectionVM playerSelectionVM);
        public abstract void enrolPlayer(TPlayer p);
        public PlayerSelectionVM playerSelectionVM { get; private set; }

        public abstract string[] puterScripts { get; }


        public ObservableCollection<TPlayer> players { get; private set; }
        public IEnumerable<TPlayer> humanPlayers { get { return players.Where(p => !p.isPuter); } }
        public IEnumerable<TPlayer> puterPlayers { get { return players.Where(p => p.isPuter); } }
        public abstract void movePlayer(object o);

        //sprites
        public abstract ObservableCollection<FixedSprite> sprites { get; }
        public abstract void doCollisions();

        public bool gameRunning;
        

        private bool hiScoreNotShownYet = false;
        private bool _gameOver = false;
        public bool gameOver 
        {
            get
            {
                //show code
                return _gameOver;
            }
            set
            {
                _gameOver = value;

                //if (hiScoreNotShownYet && _gameOver && puppets == null)
                //{
                //    hiScoreNotShownYet = false;
                //    InputBox ib = new InputBox(String.Format(@"Well done on getting to level {0}, enter your name below", level.levelNum));
                //    ib.ShowDialog();
                //    string hiScoreCode = HiScore.generateCodeFromUTF8(ib.text, level.levelNum.ToString());
                //    Clipboard.SetText(hiScoreCode);
                //    MessageBox.Show(String.Format("Your hi score code is: {0} \n It has been copied to the Clipboard. Paste the code into the online hi score board to impress your friends!", hiScoreCode), "");
                //}

            }
        }

        int countDownNumberToShow;
        Timer countdownTimer;
        protected ICommand _restartGameCommand;
        public ICommand restartGame
        {
            get
            {
                if (_restartGameCommand == null)
                    this._restartGameCommand = new DelegateCommand((o) => { doRestartGame(); });
                return this._restartGameCommand;
            }
        }

        protected ICommand _showInstructionsCommand;
        public ICommand showInstructions
        {
            get
            {
                if (_showInstructionsCommand == null)
                    this._showInstructionsCommand = new DelegateCommand((o) => 
                    {
                        Window browser = new HTMLViewer(HTMLInstructions);
                        browser.Show();
                    });
                return this._showInstructionsCommand;
            }
        }


        protected ICommand _deletePlayerCommand;
        public ICommand deletePlayer
        {
            get
            {
                if (_deletePlayerCommand == null)
                    this._deletePlayerCommand = new DelegateCommand((o) =>
                    {
                        TPlayer p = o as TPlayer;
                        if (o != null)
                        {
                            if (!p.isPuter)
                                keyActions.Push(p.keyActions);
                            players.Remove(p);
                        }
                    });
                return this._deletePlayerCommand;
            }
        }
        
        protected ICommand _addPlayerCommand;
        public ICommand addPlayer
        {
            get
            {
                if (_addPlayerCommand == null)
                    this._addPlayerCommand = new DelegateCommand((o) => 
                    {
                        TPlayer p = createPlayerFromSelection(playerSelectionVM);
                        if (p != null)
                            enrolPlayer(p);
                    });
                return this._addPlayerCommand;
            }
        }

        public abstract double[] playerStartingPoint(int index);
        public abstract void resetPlayer(TPlayer p);

        public void doRestartGame()
        {



            if (players != null)
                foreach (TPlayer p in players)
                    resetPlayer(p);

            gameRunning = false;

            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
            }
            countdownTimer = new Timer(1000);
            countDownNumberToShow = 3;
            countdownTimer.Elapsed += (s, e) => 
            { 
                countDownNumberToShow--;
                if (countDownNumberToShow < 0)
                {
                    gameRunning = true;
                    countDownNumberToShow = 3;
                    countdownTimer.Stop();
                    countdownTimer = null;
                }
            };
            countdownTimer.Start();
        }

        bool isProcessingNextMoves = false;
        public WriteableBitmap bitmap
        {
            get
            {
                foreach (FixedSprite s in sprites.Concat(players))
                    _bitmap.Blit(Screen.worldToScreen(s), s.bitmap, s.sourceRect, System.Windows.Media.Colors.White, WriteableBitmapExtensions.BlendMode.Alpha);

                if (gameRunning)
                {
                    if (!isProcessingNextMoves)
                    {
                        isProcessingNextMoves = true;
                        //get keys. tasks cannot do keyboard check since they are background threads.
                        keysDownMomentary = keysToCheckPermanent.Where(k => Keyboard.IsKeyDown(k)).ToArray();

                        //tasks for scripts
                        Task[] playerMoveTasks = players.Select(p => Task.Factory.StartNew(movePlayer, p)).ToArray();

                        //collisions task at the end
                        if (playerMoveTasks.Any())
                            Task.Factory.ContinueWhenAll(playerMoveTasks, (t) =>
                            {
                                doCollisions();
                                isProcessingNextMoves = false;
                            });
                    }
                    else
                    {
                        Debug.WriteLine("frame skipped" + DateTime.Now.Ticks);
                    }
                }
                else
                {
                    FixedSprite s = countdownNumbers[countDownNumberToShow];
                    _bitmap.Blit(Screen.worldToScreen(s), s.bitmap, s.sourceRect, System.Windows.Media.Colors.White, WriteableBitmapExtensions.BlendMode.Alpha);
                }

                return _bitmap;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
