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
    public partial class User
    {
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
    public partial class League
    {

    }

    public class LeagueMetaData
    {
        public int LeagueID { get; set; }
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
    [MetadataType(typeof(TeamMetaData))]
    public partial class Team
    {

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

}