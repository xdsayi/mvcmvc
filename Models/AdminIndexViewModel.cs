

using mvcmvc.Data;

namespace mvcmvc.Models
{
    public class AdminIndexViewModel
    {
        public IEnumerable<Data.Blog> Blogs { get; set; }
        public IEnumerable<Data.Film> Films { get; set; }
    }
}
