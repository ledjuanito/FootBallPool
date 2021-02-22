using FootBallPool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Amazon.DynamoDBv2.DataModel;
using DynamoDBConection;
using Amazon.DynamoDBv2.DocumentModel;

namespace FootBallPool.Controllers
{
    [Authorize]
    public class LeagueController : Controller
    {
        DynamoDBContext context = new DynamoDBContext(new DynamoDBInitializer().client);

        // GET: League
        public ActionResult Index()
        {
            var model = context.Scan<League>(new ScanCondition[0]).AsEnumerable();
            ViewBag.MatchWeeks = context.Scan<MatchWeek>(new ScanCondition[0]).GroupBy(a => a.LeagueID).Select(a => new { LeagueID = a.Key, Count = a.Count() }).ToDictionary(a => a.LeagueID, b => b.Count);
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
                model.LeagueID = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid();
                context.SaveAsync<League>(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult MatchWeek(string id)
        {
            MatchWeekIndex model = new MatchWeekIndex();
            model.League = context.Load<League>(id);
            //var model = db.MatchWeeks.Include(m => m.League).Where(m => m.LeagueID == id).OrderBy(m => m.StarDate).AsEnumerable();
            List<ScanCondition> condition = new List<ScanCondition>();
            condition.Add(new ScanCondition("LeagueID", ScanOperator.Equal, id));
            model.Index = context.Scan<MatchWeek>(condition.ToArray()).OrderBy(m => m.StarDate).AsEnumerable();
            ViewBag.LeagueID = id;
            return View(model);
        }

        public ActionResult AddMatchWeek(string id)
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
                model.MatchWeekID = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid();
                context.SaveAsync<MatchWeek>(model);
                return RedirectToAction("MatchWeek", new { id = model.LeagueID });
            }
            return View(model);
        }

        //Team
        #region Teams
        public ActionResult TeamIndex(string id)
        {
            TeamIndex model = new TeamIndex();
            model.League = context.Load<League>(id);
            List<ScanCondition> condition = new List<ScanCondition>();
            condition.Add(new ScanCondition("LeagueID", ScanOperator.Equal, id));
            model.Index = context.Scan<Team>(condition.ToArray()).OrderBy(m => m.Name).ToList();
            //var model = db.Teams.Include(m => m.League).Where(m => m.LeagueID == id).OrderBy(m => m.Name).AsEnumerable();
            ViewBag.LeagueID = id;
            return View(model);
        }

        public ActionResult AddTeam(string id)
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
                model.TeamID = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid();
                context.SaveAsync<Team>(model);
                return RedirectToAction("TeamIndex", new { id = model.LeagueID });
            }
            return View(model);
        }
        #endregion

        //Fixtures
        #region Fixtures & Results
        public ActionResult Fixtures(string id)
        {
            Fixtures model = new Fixtures();
            MatchWeek matchWeek = context.Load<MatchWeek>(id);
            model.matchWeek.Add(matchWeek.MatchWeekID,matchWeek);
            List<ScanCondition> condition = new List<ScanCondition>();
            condition.Add(new ScanCondition("MatchWeekID", ScanOperator.Equal, id));
            model.Index = context.Scan<Match>(condition.ToArray());
            condition = new List<ScanCondition>();
            condition.Add(new ScanCondition("LeagueID", ScanOperator.Equal, id));
            model.Teams = context.Scan<Team>(condition.ToArray()).ToDictionary(a => a.TeamID, b => b);
            //var model= db.Matches.Include(m => m.MatchWeek).Where(m => m.MatchWeekID == id).AsEnumerable();
            //ViewBag.MatchWeekID = id;
            //int leagueId = db.MatchWeeks.Find(id).LeagueID;
            //ViewBag.Teams = db.Teams.Where(t => t.LeagueID == leagueId).ToDictionary(a => a.TeamID, b => b);
            ViewBag.allowAdd = true;
            return View(model);
        }

        public ActionResult FixturesAndResults(string id)
        {
            Fixtures model = new Fixtures();

            League league = context.Load<League>(id);
            List<ScanCondition> condition = new List<ScanCondition>();
            condition.Add(new ScanCondition("LeagueID", ScanOperator.Equal, league.LeagueID));
            model.matchWeek = context.Scan<MatchWeek>(condition.ToArray()).ToDictionary(a => a.MatchWeekID, b => b);
            string matchWeekID = model.matchWeek.Count() > 0 ? model.matchWeek.FirstOrDefault().Value.MatchWeekID : null;

            condition = new List<ScanCondition>();
            condition.Add(new ScanCondition("MatchWeekID", ScanOperator.Equal, matchWeekID));
            //var aa = context.Scan<Match>(condition.ToArray());
            //var s = aa.Count();
            //var index = from a in aa join b in model.matchWeek.Values on a.MatchWeekID equals b.MatchWeekID into all from matchweek in all select matchweek;
            //var aas = index.ToList();
            model.Index = context.Scan<Match>(condition.ToArray());

            condition = new List<ScanCondition>();
            condition.Add(new ScanCondition("LeagueID", ScanOperator.Equal, id));
            model.Teams = context.Scan<Team>(condition.ToArray()).ToDictionary(a => a.TeamID, b => b);

            //var model = db.Matches.Include(m => m.MatchWeek).Where(m => m.MatchWeek.LeagueID == id).AsEnumerable();
            //ViewBag.MatchWeekID = id;
            //ViewBag.Teams = db.Teams.Where(t => t.LeagueID == id).ToDictionary(a => a.TeamID, b => b);
            ViewBag.allowAdd = false;
            return View("~/Views/League/Fixtures.cshtml", model);
        }

        public ActionResult AddFixture(string id)
        {
            Match model = new Match();
            model.MatchWeekID = id;
            string leagueId = context.Load<MatchWeek>(id).LeagueID;
            List<ScanCondition> condition = new List<ScanCondition>();
            condition.Add(new ScanCondition("LeagueID", ScanOperator.Equal, leagueId));
            var teams = context.Scan<Team>(condition.ToArray()).OrderBy(m => m.Name);
            ViewBag.Teams = new SelectList(teams, "TeamID", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFixture(Match model)
        {
            if (ModelState.IsValid)
            {
                model.MatchID = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid();
                context.SaveAsync<Match>(model);
                //db.Matches.Add(model);
                //db.SaveChanges();
            }
            return RedirectToAction("Fixtures", new { id = model.MatchWeekID });
        }
        #endregion
    }
}