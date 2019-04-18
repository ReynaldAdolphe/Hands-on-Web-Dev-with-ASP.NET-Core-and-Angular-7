using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DVDMovie.Models;
using Microsoft.EntityFrameworkCore;
using DVDMovie.Models.BindingTargets;
using Microsoft.AspNetCore.JsonPatch;

namespace DVDMovie.Controllers
{
    [Route("api/movies")]
    public class MovieController : Controller
    {
        private DataContext context;
        public MovieController(DataContext ctx)
        {
            context = ctx;
        }

        [HttpGet("{id}")]
        public Movie GetMovie(long id)
        {
            //System.Threading.Thread.Sleep(5000);
            Movie result = context.Movies
                            .Include(m => m.Studio).ThenInclude(s => s.Movies)
                            .Include(m => m.Ratings)
                            .FirstOrDefault(m => m.MovieId == id);


            if (result != null)
            {
                if (result.Studio != null)
                {
                    result.Studio.Movies = result.Studio.Movies.Select(p =>
                        new Movie
                        {
                            MovieId = p.MovieId,
                            Name = p.Name,
                            Category = p.Category,
                            Description = p.Description,
                            Price = p.Price,
                        });
                }
                if (result.Ratings != null)
                {
                    foreach (Rating r in result.Ratings)
                    {
                        r.Movie = null;
                    }
                }
            }
            return result;
        }

        [HttpGet]
        public IActionResult GetMovies(string category, string search,
                                            bool related = false, bool metadata = false) {
        {
            IQueryable<Movie> query = context.Movies;

            if (!string.IsNullOrWhiteSpace(category))
            {
                string catLower = category.ToLower();
                query = query.Where(m => m.Category.ToLower().Contains(catLower));
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(m => m.Name.ToLower().Contains(searchLower)
                || m.Description.ToLower().Contains(searchLower));
            }
            if (related)
            {
                query = query.Include(p => p.Studio).Include(p => p.Ratings);
                List<Movie> data = query.ToList();
                data.ForEach(p =>
                {
                    if (p.Studio != null)
                    {
                        p.Studio.Movies = null;
                    }
                    if (p.Ratings != null)
                    {
                        p.Ratings.ForEach(r => r.Movie = null);
                    }
                });
                return metadata ? CreateMetadata(data) : Ok(data);
            }
            else
            {
                return metadata ? CreateMetadata(query) : Ok(query);
            }
        }
    }
        [HttpPost]
        public IActionResult CreateMovie([FromBody] MovieData pdata)
        {
            if (ModelState.IsValid)
            {
                Movie p = pdata.Movie;
                if (p.Studio != null && p.Studio.StudioId != 0)
                {
                    context.Attach(p.Studio);
                }
                context.Add(p);
                context.SaveChanges();
                return Ok(p.MovieId);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceMovie(long id, [FromBody] MovieData mData)
        {
            if (ModelState.IsValid)
            {
                Movie m = mData.Movie;
                m.MovieId = id;
                if (m.Studio != null && m.Studio.StudioId != 0)
                {
                    context.Attach(m.Studio);
                }
                context.Update(m);
                context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateMovie(long id,
           [FromBody]JsonPatchDocument<MovieData> patch)
        {
            Movie movie = context.Movies
            .Include(p => p.Studio)
            .First(p => p.MovieId == id);
            MovieData pdata = new MovieData { Movie = movie };
            patch.ApplyTo(pdata, ModelState);
            if (ModelState.IsValid && TryValidateModel(pdata))
            {
                if (movie.Studio != null && movie.Studio.StudioId != 0)
                {
                    context.Attach(movie.Studio);
                }
                context.SaveChanges();
                return Ok(movie);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(long id)
        {
            context.Movies.Remove(new Movie { MovieId = id });
            context.SaveChanges();
            return Ok(id);
        }
        private IActionResult CreateMetadata(IEnumerable<Movie> movies)
        {
            return Ok(new
            {
                data = movies,
                categories = context.Movies.Select(m => m.Category)
            .Distinct().OrderBy(m => m)
            });
        }
    }
}
