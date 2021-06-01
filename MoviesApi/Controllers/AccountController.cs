using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Common;
using MoviesApi.Core.Constants;
using MoviesApi.Core.Extensions;
using MoviesApi.Core.Helpers;
using MoviesApi.Core.Models;
using MoviesApi.Data;
using MoviesApi.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Authorize]
    [RequireHttps]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private string UserId => HttpContext.User.Claims.ToList().FirstOrDefault(x => x.Type == Constants.UserId)?.Value;
        private string Domain => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
        private MoviesContext _moviesContext;

        public AccountController(MoviesContext moviesContext)
        {
            _moviesContext = moviesContext;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] AccountRequest accountRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                    return Unauthorized();

                if (accountRequest == null)
                    return BadRequest();

                if (_moviesContext.Accounts.Any(a => a.Id == UserId))
                    return BadRequest("Account already registered");

                Account account = new()
                {
                    Id = UserId,
                    Birthday = accountRequest.Birthday,
                    Email = accountRequest.Email,
                    Name = accountRequest.Name
                };

                await _moviesContext.Accounts.AddAsync(account);
                await _moviesContext.SaveChangesAsync();
                return Created($"{Domain}/api/Account", account);
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error registering user {UserId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult> UpdateProfile([FromBody] AccountRequest accountRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                    return Unauthorized();

                if (accountRequest == null)
                    return BadRequest();

                Account existingAccount = await _moviesContext.Accounts.AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == UserId);

                if (existingAccount == null)
                    return BadRequest("Account does not exist");

                existingAccount.Email = accountRequest.Email;
                existingAccount.Birthday = accountRequest.Birthday;
                existingAccount.Name = accountRequest.Name;

                _moviesContext.Accounts.Update(existingAccount);
                await _moviesContext.SaveChangesAsync();
                return Accepted();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error registering user {UserId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult<Account>> GetAccount()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                    return Unauthorized();

                Account existentAccount = await _moviesContext.Accounts.FirstOrDefaultAsync(a => a.Id == UserId);

                if (existentAccount == null)
                    return Unauthorized("User not registered");

                return existentAccount;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting account for user {UserId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetReviews")]
        public ActionResult<ICollection<Review>> GetReviews(string accountId = null, int max = 100, int offset = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                    return Unauthorized();

                accountId ??= UserId;

                return _moviesContext.Reviews.Where(r => r.AccountId == accountId)
                                             .AsNoTracking()
                                             .Skip(offset)
                                             .Take(max)
                                             .ToHashSet();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error getting reviews for user {accountId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("AddToWatchList")]
        public async Task<ActionResult> AddToWatchList(string[] movieIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                    return Unauthorized();

                if (!_moviesContext.Accounts.Any(a => a.Id == UserId))
                    return Unauthorized("User not registered");

                if (movieIds == null || movieIds.Length == 0)
                    return BadRequest();

                Account account = await _moviesContext.Accounts.Include(a => a.Watchlist)
                    .AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == UserId);

                if (account == null)
                    return BadRequest("Account not found");

                foreach (string movieId in movieIds)
                {
                    if (!MovieHelper.ConvertIdToInt(movieId, out int idAsInt)) continue;
                    Movie movie = _moviesContext.Movies.FirstOrDefault(m => m.Id == idAsInt);
                    if (movie != null)
                        account.Watchlist.Add(movie);
                }
                await _moviesContext.SaveChangesAsync();
                return Accepted();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error removing from watchlist; userId {UserId}; movies {movieIds}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("watchlist")]
        public ActionResult<HashSet<Movie>> GetWatchlist(int max = 100, int offset = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                    return Unauthorized();

                if (!_moviesContext.Accounts.Any(a => a.Id == UserId))
                    return Unauthorized("User not registered");

                HashSet<Movie> watchlist = _moviesContext.Accounts
                    .Where(a => a.Id == UserId)
                    .Include(a => a.Watchlist)
                    .ThenInclude(a => a.TotalRatings)
                    .SelectMany(a => a.Watchlist)
                    .OrderBy(m => m.Id)
                    .Skip(offset)
                    .Take(max)
                    .ToHashSet();

                watchlist.ForEach(m => m.IsInMyWatchlist = true);

                return watchlist;
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error get watchlist; userId {UserId}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("RemoveFromWatchList")]
        public async Task<ActionResult> RemoveFromWatchlist(string[] movieIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                    return Unauthorized();

                if (!_moviesContext.Accounts.Any(a => a.Id == UserId))
                    return Unauthorized("User not registered");

                if (movieIds == null || movieIds.Length == 0)
                    return BadRequest();

                Account account = await _moviesContext.Accounts.Include(a => a.Watchlist)
                    .AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == UserId);

                if (account == null)
                    return BadRequest("Account not found");

                foreach (string movieId in movieIds)
                {
                    if (!MovieHelper.ConvertIdToInt(movieId, out int idAsInt)) continue;
                    Movie movie = account.Watchlist.FirstOrDefault(m => m.Id == idAsInt);
                    if (movie != null)
                        account.Watchlist.Remove(movie);
                }
                await _moviesContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Default.Error($"Error removing from watchlist; userId {UserId}; movies {movieIds}", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("AddToFavouriteList")]
        public async Task<ActionResult> AddToFavouriteList(int[] personIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                    return Unauthorized();

                if (!_moviesContext.Accounts.Any(a => a.Id == UserId))
                    return Unauthorized("User not registered");

                if (personIds == null || personIds.Length == 0)
                    return BadRequest();

                Account account = await _moviesContext.Accounts.Include(a => a.FavouritePeople)
                    .AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == UserId);

                if (account == null)
                    return BadRequest("Account not found");

                foreach (int personId in personIds)
                {
                    Person person = _moviesContext.People.FirstOrDefault(p => p.Id == personId);
                    if (person != null)
                        account.FavouritePeople.Add(person);
                }
                await _moviesContext.SaveChangesAsync();
                return Accepted();
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error adding to favourite actors list", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("RemoveFromFavouriteList")]
        public async Task<ActionResult> RemoveFromFavouriteList(int[] personIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                    return Unauthorized();

                if (!_moviesContext.Accounts.Any(a => a.Id == UserId))
                    return Unauthorized("User not registered");

                if (personIds == null || personIds.Length == 0)
                    return BadRequest();

                Account account = await _moviesContext.Accounts.Include(a => a.FavouritePeople)
                    .AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == UserId);

                if (account == null)
                    return BadRequest("Account not found");

                foreach (int personId in personIds)
                {
                    Person person = _moviesContext.People.FirstOrDefault(p => p.Id == personId);
                    if (person != null)
                        account.FavouritePeople.Remove(person);
                }
                await _moviesContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Default.Error("Error removing from favourite actors list", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
