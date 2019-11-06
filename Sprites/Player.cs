using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Panic
{
    public abstract class Player : Projectile, INotifyPropertyChanged
    {
        public Color color { get; protected set; }
        public System.Windows.Media.Brush brush { get; private set; } //needed for score numbers

        public Func<string, string> script;
        public bool isPuter { get; private set; }
        public string playerCodeString { get; set; }
        public Key[] keyActions { get; protected set; }

        public Player(Color paramColor,
            List<WriteableBitmap> paramBitmap,
            double[] paramXY, double[] paramWorldWidthHeight,
            int paramLives,
            string paramCodeString,
            bool paramIsPuter,
            Key[] paramKeyActions,
            double paramWorldToScreenOffsetX,
            double paramWorldToScreenOffsetY)
            : base(paramBitmap, paramXY, paramWorldWidthHeight, 0, 0,
            paramWorldToScreenOffsetX,
            paramWorldToScreenOffsetY )
        {
            xVelocity = 0;
            yVelocity = 0;
            isPuter = paramIsPuter;
            _scoreOrLives = paramLives;

            color = paramColor;
            brush = new System.Windows.Media.SolidColorBrush(color.tomediacolor());

            playerScriptON = paramIsPuter;
            playerCodeString = paramCodeString;
            keyActions = paramKeyActions;
            
            if (isPuter)
                doUpdateBehaviour();
        }


        public string playerScriptONAsString
        {
            get { return playerScriptON ? "Script ON" : "Script OFF"; }
        }


        public abstract void move(Key[] keysDownMomentary);

        protected ICommand _updateBehaviourCommand;
        public ICommand updateBehaviour
        {
            get
            {
                if (_updateBehaviourCommand == null)
                    this._updateBehaviourCommand = new DelegateCommand(
                            (o) =>
                            {
                                doUpdateBehaviour();
                            }
                        );
                return this._updateBehaviourCommand;
            }
        }

        public void doUpdateBehaviour()
        {
            Task.Factory.StartNew(() =>
            {
                Func<string, string> newScript = PythonScript.Create(playerCodeString, "move", 1);
                if (newScript != null)
                {
                    script = newScript;
                    playerScriptON = true;
                }
            });
        }

        protected bool _playerScriptON;
        public bool playerScriptON
        {
            get { return _playerScriptON; }
            set
            {
                _playerScriptON = value;
                RaisePropertyChanged("playerScriptON");
                RaisePropertyChanged("playerScriptONAsString");
            }
        }

        protected int _scoreOrLives;
        public int scoreOrLives
        {
            get { return _scoreOrLives; }
            set { _scoreOrLives = value; RaisePropertyChanged("scoreOrLives"); }
        }

        public string coordinatesFromFrontCentre(double[] coordsXY)
        {
            string retVal = "";
            for (int i = 0; i < coordsXY.Length / 2; i += 2)
            {
                retVal += Math.Floor(coordsXY[i] - worldX).ToString() + ",";
            }
            return retVal.Substring(0, retVal.Length-1);
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
