using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootBallPool.Models
{
    //User 
    #region User
    [MetadataType(typeof(UserMetaData))]
    //[DynamoDBTable("User")]
    public class User
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserInfo Info
        {
            get
            {
                return new UserInfo(this);
            }
        }

        public class UserInfo
        {
            public User user;
            public string FullName { get { return string.Format("{0} {1}", user.Name, user.LastName); } }
            //public string Role { get { return user.Role.Name; } }

            public UserInfo(User user)
            {
                this.user = user;
            }

            //public bool HasPermission(string controller = "Home", string action = "Index")
            //{
            //    return this.user.HasPermission(controller, action);
            //}

            //public static bool UserHasPermission(string userId, string controller = "Home", string action = "Index")
            //{
            //    var usr = new FootballPoolEntities().Users.Find(userId);
            //    return usr != null && usr.HasPermission(controller, action);
            //}
        }
    }
    public class UserMetaData
    {
        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [MaxLength(50, ErrorMessage = "La longitud máxima es de 50 caracteres")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        //[Required(ErrorMessage = "El email es obligatorio")]
        [MaxLength(50, ErrorMessage = "La longitud máxima es de 50 caracteres")]
        public string Email { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(50, ErrorMessage = "La longitud máxima es de 50 caracteres")]
        public string Name { get; set; }
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MaxLength(20, ErrorMessage = "La longitud máxima es de 20 caracteres")]
        public string Password { get; set; }
        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "El usuario es obligatorio")]
        [MaxLength(128, ErrorMessage = "La longitud máxima es de 128 caracteres")]
        public string UserID { get; set; }
    }

    public class ChangePass
    {
        [Display(Name = "Contraseña Antigua")]
        [Required(ErrorMessage = "La Contraseña Antigua es obligatoria.")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Contraseña Nueva")]
        [Required(ErrorMessage = "La Contraseña Nueva es obligatoria.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = " Confirmar Contraseña Nueva")]
        [Required(ErrorMessage = "La Confirmación de Contraseña Nueva es obligatoria.")]
        [Compare(otherProperty: "NewPassword", ErrorMessage = "La Contraseña Nueva no coincide")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
    #endregion

    //League
    #region League
    [MetadataType(typeof(LeagueMetaData))]
    public class League
    {
        public string LeagueID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }

    public class LeagueMetaData
    {
        public string LeagueID { get; set; }
        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "Max length = 50 characters")]
        public string Name { get; set; }
        [Display(Name = "Image URL")]
        [MaxLength(250, ErrorMessage = "Max length = 250 characters")]
        public string Image { get; set; }

    }
    #endregion

    //Team
    #region Team
    public class TeamIndex
    {
        public IEnumerable<Team> Index { get; set; }
        public League League { get; set; }

        public TeamIndex()
        {
            Index = new List<Team>();
            League = new League();
        }
    }

    [MetadataType(typeof(TeamMetaData))]
    public class Team
    {
        public string TeamID { get; set; }
        public string LeagueID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public League League { get; set; }
    }
    public class TeamMetaData
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Image URL")]
        [MaxLength(250, ErrorMessage = "Max length = 250 characters")]
        public string Image { get; set; }
    }
    #endregion

    public class Fixtures
    {
        public IEnumerable<Match> Index { get; set; }
        public Dictionary<string,MatchWeek> matchWeek { get; set; }
        public Dictionary<string,Team> Teams { get; set; }
        public Fixtures()
        {
            Index = new List<Match>();
            matchWeek = new Dictionary<string,MatchWeek>();
            Teams = new Dictionary<string, Team>();
        }
    }

    public class PoolMember
    {
        public string PoolID { get; set; }
        public string UserID { get; set; }

    }

    public class Pool
    {
        public string PoolID { get; set; }
        public string Name { get; set; }
        public string LeagueID { get; set; }

    }

    public class MatchWeekIndex
    {
        public IEnumerable<MatchWeek> Index { get; set; }
        public League League { get; set; }


        public MatchWeekIndex()
        {
            Index = new List<MatchWeek>();
            League = new League();
        }
    }

    public class MatchWeek
    {
        public string MatchWeekID { get; set; }
        public string Name { get; set; }
        public string LeagueID { get; set; }
        public System.DateTime StarDate { get; set; }
        public System.DateTime EndDate { get; set; }

        public League League { get; set; }
    }

    public class MatchPool
    {
        public string MatchPoolID { get; set; }
        public string MatchWeekID { get; set; }
        public string UserID { get; set; }
        public Nullable<int> LocalScore { get; set; }
        public Nullable<int> VisitorScore { get; set; }
        public int Score { get; set; }
        public string IsSaved { get; set; }
        public string PoolID { get; set; }
        public string MatchID { get; set; }

    }

    public class Match
    {
        public string MatchID { get; set; }
        public string MatchWeekID { get; set; }
        public string LocalTeam { get; set; }
        public string VisitorTeam { get; set; }
        public int LocalScore { get; set; }
        public int VisitorScore { get; set; }

    }
}