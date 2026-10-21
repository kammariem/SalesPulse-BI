# Vérifications manuelles avant livraison

## Lancement

- [ ] `dotnet --version` affiche .NET SDK 8.x.
- [ ] `cd backend && dotnet restore && dotnet build` réussit.
- [ ] `cd frontend && npm ci && npm run build` réussit.
- [ ] `dotnet run --launch-profile http` et `npm start` démarrent sans erreur.
- [ ] `GET /api/health` répond correctement.
- [ ] `GET /api/ssas/test` affiche `isConnected: true` avec le bon serveur/base.

## Contrôles métier avec ton propre cube

- [ ] Toutes les mesures de `backend/Constants/CubeDefinition.cs` sont présentes dans `CUBE_EDB`.
- [ ] Dashboard : valider les valeurs KPI et le chargement des quatre graphiques.
- [ ] Sales : années, trimestres, mois et drill-down par dimension.
- [ ] Products, Customers, Geography, Employees : vérifier un classement avec des totaux connus.
- [ ] Dimensions : `top=5` renvoie au plus **5** membres.
- [ ] En cas de serveur SSAS arrêté, les pages affichent une erreur visible, pas un graphique vide présenté comme un succès.
- [ ] Pour des valeurs négatives et zéro, les graphiques barres/lignes restent valides ; pour le diagramme circulaire, seules les valeurs strictement positives représentent des parts.
- [ ] Vérifier le temps de réponse avec des hiérarchies de nombreuses valeurs (Top N est appliqué après récupération pour certains endpoints).

## Vérification sécurité / GitHub

- [ ] `backend/appsettings.Local.json` n'est pas suivi par Git.
- [ ] Aucun mot de passe, token, adresse privée ou donnée client n'est committé.
- [ ] Aucune sortie `bin`, `obj`, `node_modules`, `dist` n'est committée.
- [ ] Captures d'écran anonymisées, basées sur des données autorisées.

**Statut du livrable :** revue statique du code et nettoyage. Le SDK .NET 8 et le cube SSAS ne sont pas disponibles dans l'environnement de préparation ; l'installation des dépendances Angular a également échoué faute d'accès au registre npm (erreur réseau `EAI_AGAIN`). L'exécution du backend et la compilation Angular restent donc à confirmer sur ton PC.
