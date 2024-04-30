using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    /// <summary>
    /// Cette énumération sera utilisée pour savoir à quel stade est un dossier d'inventaire.
    /// </summary>
    public enum InventoryDirectorySatut
    {
       Opened,       //Dès la création du dossier d'inventaire, son statut passe à ouvert
       InProgess,   //Dès la saisie de la première ligne d'inventaire, le statut passe à Encours
       Closed,     //Dès la fermeture du dossier d'inventaire, le statut passe à fermé
    }
}
