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
    public class LeagueController : Controller
    {
        FootballPoolEntities db = new FootballPoolEntities();
        // GET: League
        public ActionResult Index()
        {
            var model = db.Leagues.AsEnumerable();
            return View(model);
        }

        public ActionResult AddLeague()
        {
            League model = new League();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLeague(League model)
        {
            if (ModelState.IsValid)
            {
                db.Leagues.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult MatchWeek(int id)
        {
            var model = db.MatchWeeks.Include(m => m.League).Where(m => m.LeagueID == id).OrderBy(m => m.StarDate).AsEnumerable();
            ViewBag.LeagueID = id;
            return View(model);
        }

        public ActionResult AddMatchWeek(int id)
        {
            var model = new MatchWeek();
            model.LeagueID = id;
            model.StarDate = DateTime.Now;
            model.EndDate = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMatchWeek(MatchWeek model)
        {
            if (ModelState.IsValid)
            {
                db.MatchWeeks.Add(model);
                db.SaveChanges();

                return RedirectToAction("MatchWeek", new { id = model.LeagueID });
            }
            return View(model);
        }

        //Team
        #region Teams
        public ActionResult TeamIndex(int id)
        {
            var model = db.Teams.Include(m => m.League).Where(m => m.LeagueID == id).OrderBy(m => m.Name).AsEnumerable();
            ViewBag.LeagueID = id;
            return View(model);
        }

        public ActionResult AddTeam(int id)
        {
            Team model = new Team();
            model.LeagueID = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTeam(Team model)
        {
            if (ModelState.IsValid)
            {
                db.Teams.Add(model);
                db.SaveChanges();

                return RedirectToAction("TeamIndex",new { id = model.LeagueID });
            }
            return View(model);
        }
        #endregion

        //Fixtures
        #region Fixtures & Results
        public ActionResult Fixtures(int id)
        {
            var model= db.Matches.Include(m => m.MatchWeek).Where(m => m.MatchWeekID == id).AsEnumerable();
            ViewBag.MatchWeekID = id;
            int leagueId = db.MatchWeeks.Find(id).LeagueID;
            ViewBag.Teams = db.Teams.Where(t => t.LeagueID == leagueId).ToDictionary(a => a.TeamID, b => b);
            ViewBag.allowAdd = true;
            return View(model);
        }
        
        public ActionResult FixturesAndResults(int id)
        {
            var model = db.Matches.Include(m => m.MatchWeek).Where(m => m.MatchWeek.LeagueID == id).AsEnumerable();
            ViewBag.MatchWeekID = id;
            ViewBag.Teams = db.Teams.Where(t => t.LeagueID == id).ToDictionary(a => a.TeamID, b => b);
            ViewBag.allowAdd = false;
            return View("~/Views/League/Fixtures.cshtml", model);
        }

        public ActionResult AddFixture(int id)
        {
            Match model = new Match();
            model.MatchWeekID = id;
            int leagueId = db.MatchWeeks.Find(id).LeagueID;
            ViewBag.Teams = new SelectList(db.Teams.Where(t => t.LeagueID == leagueId).OrderBy(t => t.Name), "TeamID", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFixture(Match model)
        {
            if (ModelState.IsValid)
            {
                db.Matches.Add(model);
                db.SaveChanges();
            }
            return RedirectToAction("Fixtures", new { id = model.MatchWeekID });
        }
        #endregion
    }
}