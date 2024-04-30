using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FatSod.Supply.Entities
{
    [Serializable]
    public class NotificationSetting
    {
        public int NotificationSettingId { get; set; }
        [Required]
        public string FrenchMessage { get; set; }
        [Required]
        public string EnglishMessage { get; set; }
        [Required]
        public NotificationType NotificationType { get; set; }
        [NotMapped]
        public string NotificationTypeUI { get; set; }
    }

    public enum NotificationType
    {
        NONE,
        RDV_CONSULTATION,
        DELIVERY,
        INSURANCE_DELIVERY,
        BIRTHDAY, // Envoyer le jour d'anniversaire d'un client
        COMMAND_GLASS, // Envoyer aux clients de verres de commande pour informer sur les verres
        PURCHASE, // Envoyer au client qui ont faits au moins un achat sur une periode(cas de Tombola annuel)
        GENERAL // Un message envoye a un utilisateur 

    }
}
