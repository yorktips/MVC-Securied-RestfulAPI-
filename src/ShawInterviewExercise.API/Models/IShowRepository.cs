using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShawInterviewExercise.API.Models
{
    interface IShowRepository
    {
        IEnumerable<Show> GetAllShows();
        string AddShow(Show show);
        Show GetShowById(int id);
        Show EditShow(Show show);
        void DeleteShow(int id);
    }
}
