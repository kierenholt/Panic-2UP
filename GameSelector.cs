using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Panic
{
    public class GameSelector : INotifyPropertyChanged
    {
        private MainWindow _window;
        public GameSelector() 
        { 
            _currentGame = new RaceGame();
        }

        private string[] _gameNames = new string[] {"Race","Pong"};
        public string[] gameNames { get { return _gameNames; } }

        private int _selectedIndex  = 0;
        public int selectedIndex 
        { 
            get { return _selectedIndex; } 
            set 
            {
                _selectedIndex = value;
                if (_selectedIndex == 1)
                    _currentGame = null;//new PongGame();
                else
                    _currentGame = new RaceGame();
                RaisePropertyChanged("currentGame"); 
            } 
        }

        public object _currentGame = null;
        public object currentGame 
        { 
            get 
            {
                return _currentGame;
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
