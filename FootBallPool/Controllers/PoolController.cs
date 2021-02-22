using FootBallPool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace FootBallPool.Controllers
{
    [Authorize]
    public class PoolController : Controller
    {
        FootballPoolEntities db = new FootballPoolEntities();
        // GET: Pool
        public ActionResult Index()
        {
            var user = (FootBallPool.Models.User.UserInfo)Session["user"];
            var model = db.PoolMembers.Include(m => m.Pool).Where(m => m.UserID == user.user.UserID).OrderBy(m => m.Pool.Name).AsEnumerable();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JoinPool(int id)
        {
            var user = (FootBallPool.Models.User.UserInfo)Session["user"];
            try
            {
                PoolMember member = new PoolMember();
                member.PoolID = id;
                member.UserID = user.user.UserID;
                db.PoolMembers.Add(member);
                db.SaveChanges();

                AddMatchPool(id);
            }
            catch (Exception e)
            {
                throw new Exception("Wrong ID");
            }
            return RedirectToAction("Index");
        }

        public void AddMatchPool(int id)
        {
            var user = (FootBallPool.Models.User.UserInfo)Session["user"];
            int leagueId = db.Pools.Find(id).LeagueID;
            var matchweek = db.MatchWeeks.Include(m => m.Matches).Where(m => m.LeagueID == leagueId).ToList();
            foreach (var item in matchweek)
            {
                foreach (var game in item.Matches)
                {
                    MatchPool pool = new MatchPool();
                    pool.MatchWeekID = item.MatchWeekID;
                    pool.MatchID = game.MatchID;
                    pool.UserID = user.user.UserID;
                    pool.Score = 0;
                    pool.IsSaved = "N";
                    pool.PoolID = id;

                    try
                    {
                        db.MatchPools.Add(pool);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error Inserting MatchPool");
                    }
                }
            }
        }
        public ActionResult MatchweekPool(int leagueId, int poolId)
        {
            var matchWeek = db.MatchWeeks.Where(m => m.LeagueID == leagueId && DateTime.Now < m.StarDate);
            IEnumerable<MatchPool> model = new List<MatchPool>();

            var modelPool = db.MatchPools.Where(m => m.MatchWeekID == model.First().MatchWeekID);
            return View(model);
        }
        public ActionResult AddPool()
        {
            return View();
        }
        public ActionResult AddMembers(int id)
        {
            return View();
        }
    }
}