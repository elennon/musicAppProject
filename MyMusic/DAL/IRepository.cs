using MyMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.DAL
{
    public interface IRepository
    {
        ObservableCollection<RadioGenre> GetRadioGenres();
        ObservableCollection<RadioStream> GetRadioStations();
        void BackUpDb();
        void GetApiFillDB();
        void fillDB();
        
    }
}
