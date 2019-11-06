using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Panic
{
    public class PlayerSelectionVM: INotifyPropertyChanged
    {
        public PlayerSelectionVM(int paramNumPuterLevels, Bitmap paramPlayerSpriteMap) 
        {
            rotatingSprite = new Projectile(paramPlayerSpriteMap.toFrames(0, 0, 32, 32, 8), new[] { 0d, 0d }, new[] {0d, 0d}, 0,0, 0, 0 );
            playerSpriteMap = paramPlayerSpriteMap;
            _selectedColor = System.Windows.Media.Colors.Black;
 
            _numPuterLevels = paramNumPuterLevels;
            rotationTimer = new Timer(100);
            rotationTimer.Elapsed += (s, e) => 
            { 
                rotatingSprite.rotation++;
                rotatingSprite.updateRotation();
                RaisePropertyChanged("rotatingSprite");
            };
        }

        
        private int _vehicle;
        public int vehicle 
        {
            get { return _vehicle; }
            set
            {
                if (_vehicle != value)
                {
                    _vehicle = value;
                    rotationTimer.Stop();
                    rotatingSprite = new Projectile(playerSpriteMap.toFrames(0, _vehicle*32, 32, 32, 8).colorized(_selectedColor.todrawingcolor()), new[] {0d, 0d}, new[] {0d, 0d}, 0, 0,0,0);
                    RaisePropertyChanged("rotatingSprite");
                    rotationTimer.Start();
                }

            }
        }

        private Bitmap playerSpriteMap;
        private int _numPuterLevels;
        public int selectedPuterLevelZeroIsHuman { get; set; }
        public string[] puterPlayerLevelComboList 
        { 
            get { return new string[] { "Human" }.Concat(Enumerable.Range(0, _numPuterLevels).Select(s => "Puter " + s.ToString())).ToArray(); } 
        }


        private Timer rotationTimer;

        private bool _expanded;
        public bool expanded 
        {
            get { return _expanded; }
            set 
            { 
                _expanded = value;
                rotationTimer.Enabled = value;
            }
        }

        private System.Windows.Media.Color _selectedColor;
        public System.Windows.Media.Color selectedColor 
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    rotationTimer.Stop();
                    rotatingSprite = new Projectile(playerSpriteMap.toFrames(0, _vehicle * 32, 32, 32, 8).colorized(_selectedColor.todrawingcolor()), new[] { 0d, 0d }, new[] {0d, 0d}, 0, 0,0,0);
                    RaisePropertyChanged("rotatingSprite");
                    rotationTimer.Start();
                }
            }
        }

        public Projectile rotatingSprite { get; private set; }

        protected ICommand _nextVehicleCommand;
        public ICommand nextVehicle
        {
            get
            {
                if (_nextVehicleCommand == null)
                    this._nextVehicleCommand = new DelegateCommand((o) =>
                    {
                        vehicle = (vehicle + 1).mod(8);
                    });
                return this._nextVehicleCommand;
            }
        }


        protected ICommand _previousVehicleCommand;
        public ICommand previousVehicle
        {
            get
            {
                if (_previousVehicleCommand == null)
                    this._previousVehicleCommand = new DelegateCommand((o) =>
                    {
                        vehicle = (vehicle-1).mod(8);
                    });
                return this._previousVehicleCommand;
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
