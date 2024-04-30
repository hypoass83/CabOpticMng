Pour utiliser ce modèle avec l’authentification Windows Azure, consultez la page http://go.microsoft.com/fwlink/?LinkID=267940.

Sinon, pour utiliser ce modèle avec l’authentification Windows, consultez les instructions ci-dessous :

Hébergement sur IIS Express :
1. Dans l’Explorateur de solutions, cliquez avec le bouton droit sur le projet pour le sélectionner.
2. Si le volet Propriétés n’est pas ouvert, ouvrez-le (F4).
3. Dans le volet Propriétés de votre projet :
   a) Attribuez la valeur « Désactivé » à l’option « Authentification anonyme ».
   b) Attribuez la valeur « Activé » à l’option « Authentification Windows ».

Hébergement sur IIS 7 ou version ultérieure :
1. Ouvrez le Gestionnaire des services Internet et accédez à votre site Web.
2. Dans Affichage des fonctionnalités, double-cliquez sur Authentification.
3. Dans la page Authentification, sélectionnez Authentification Windows. Si l’authentification Windows
   n’apparaît pas dans les options, vous devrez vérifier que l’authentification Windows
   est installée sur le serveur.

      Pour activer l’authentification Windows sur Windows :
      a) Dans le Panneau de configuration, ouvrez « Programmes et fonctionnalités ».
      b) Sélectionnez « Activer ou désactiver des fonctionnalités Windows ».
      c) Accédez à Services Internet (IIS) > Services World Wide Web > Sécurité
         et vérifiez que le nœud Authentification Windows est sélectionné.

      Pour activer l’authentification Windows sur Windows Server :
      a) Dans le Gestionnaire de serveur, sélectionnez Serveur Web (IIS) et cliquez sur Ajouter des services de rôle.
      b) Accédez à Serveur Web > Sécurité
         et vérifiez que le nœud Authentification Windows est sélectionné.

4. Dans le volet Actions, cliquez sur Activer pour utiliser l’authentification Windows.
5. Dans la page Authentification, sélectionnez Authentification anonyme.
6. Dans le volet Actions, cliquez sur Désactiver pour désactiver l’authentification anonyme.