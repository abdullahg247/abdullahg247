using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataModels.Annotations;

namespace DataModels
{
    public class RegexData : INotifyPropertyChanged, IEntity
    {
        private int _documentId;

        public int DocumentId
        {
            get => _documentId;
            set
            {
                _documentId = value;
                OnPropertyChanged(nameof(DocumentId));
            }
        }

        private TimeSpan _timeSpan;

        public TimeSpan TimeSpan
        {
            get => _timeSpan;
            set
            {
                _timeSpan = value;
                OnPropertyChanged(nameof(TimeSpan));
            }
        }

        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        private string _customPatteren;

        public string CustomPatteren
        {
            get => _customPatteren;
            set
            {
                _customPatteren = value;
                OnPropertyChanged(nameof(CustomPatteren));
            }
        }


        private Regex _regex;

        public Regex Regex
        {
            get { return _regex; }
            set { _regex = value; }
        }



        private ObservableCollection<Match> _gorupCollection;

        public ObservableCollection<Match> Matches
        {
            get => _gorupCollection;
            set
            {
                _gorupCollection = value;
                OnPropertyChanged(nameof(Matches));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
