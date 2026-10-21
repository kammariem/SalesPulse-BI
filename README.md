# SalesPulse BI — Analyse décisionnelle des ventes

**Angular 16 · Chart.js · ASP.NET Core 8 · SSAS OLAP · MDX · ADOMD.NET**

Application de Business Intelligence permettant l'analyse des ventes par période, produits, clients, géographie et collaborateurs. Elle présente des indicateurs clés de performance, des graphiques et des tableaux de synthèse alimentés par un cube SQL Server Analysis Services.

> **Données non incluses :** l'archive d'origine ne fournit pas le projet SSIS, la base / l'entrepôt SQL Server, ni le projet de déploiement du cube SSAS `CUBE_EDB`. Le frontend ne contient **pas** de données de démonstration fabriquées : pour afficher les KPI réels, un cube SSAS compatible est nécessaire.

## Structure

```text
SalesPulse-BI-Improved/
├── frontend/          # Angular 16 et Chart.js
├── backend/           # API ASP.NET Core 8 et MDX
├── docs/              # Architecture, installation, vérifications
├── .gitignore         # Exclusions Git
└── README.md
```

## Installation rapide (Windows)

**Prérequis :** Node.js compatible Angular 16 (Node 18 LTS recommandé), npm, .NET SDK 8, SQL Server Analysis Services Multidimensional avec un cube `CUBE_EDB` conforme aux mesures / dimensions déclarées dans `backend/Constants/CubeDefinition.cs`. L'authentification intégrée Windows doit avoir accès au cube. Sur d'autres systèmes, la connexion intégrée `SSPI` n'est pas garantie.

1. Copier `backend/appsettings.Local.example.json` vers `backend/appsettings.Local.json` (ne jamais committer ce dernier).
2. Dans `backend/appsettings.Local.json`, remplacer `YOUR_SSAS_SERVER` par le nom de ton instance SSAS. Vérifier la base / le nom du cube (et les mesures du projet).
3. Démarrer l'API :

```powershell
cd backend
dotnet restore
dotnet run --launch-profile http
```

4. Dans un **deuxième terminal**, démarrer le frontend :

```powershell
cd frontend
npm ci
npm start
```

5. Ouvrir `http://localhost:4200`. La configuration par défaut redirige `/api` vers `http://localhost:5084` via le proxy Angular. Tester d'abord `http://localhost:5084/api/ssas/test` et Swagger `http://localhost:5084/swagger`.

Il est aussi possible de configurer la connexion localement via la variable d'environnement .NET `Ssas__ConnectionString` (par exemple pour CI ou déploiement). `appsettings.Local.json` est une solution de développement, à ne jamais publier avec des identifiants.

## Pages et fonctionnalités

- **Dashboard :** KPI, évolution des ventes, top produits, clients, pays et employés.
- **Sales :** séries temporelles et exploration dimensionnelle via niveaux de hiérarchies.
- **Products / Customers / Geography / Employees :** comparaisons et classements.
- **Dimensions :** inspection des membres des dimensions et de leur `MEMBER_UNIQUE_NAME`.
- **API :** contrôleurs REST, accès ADOMD.NET à SSAS, requêtes MDX, Swagger en développement.

Le drill-down passe actuellement par **la sélection de la dimension et du niveau**, et non par des clics directs sur les segments des graphiques.

## Sécurité et fiabilité

- La chaîne de connexion SSAS n'est plus livrée avec un nom de poste personnel.
- L'exécution de **MDX libre est désactivée** (`AllowRawMdxInDevelopment: false`). Même si réactivée en local, elle n'est disponible qu'en mode Development. Ne l'active pas sur une instance partagée.
- Les erreurs de requêtes sont signalées par des statuts HTTP et apparaissent en frontend, au lieu d'être converties en graphiques vides « réussis ».
- Les résultats nuls sont distingués des `0` dans les tableaux. Les graphiques de type barres/lignes affichent des données valides égales à `0` ou négatives.

## Vérifications et limites

Voir [docs/VALIDATION.md](docs/VALIDATION.md) pour la checklist de vérification. Une compilation complète du backend et une validation métier exigent .NET 8 et un serveur SSAS accessibles. Le volume des membres SSAS est récupéré avant limitation Top N dans certaines requêtes pour éviter des erreurs `TOPCOUNT` sur des hiérarchies utilisateur profondes : à optimiser selon la taille réelle du cube et les tests de performance.

## Avant la publication GitHub

Vérifier l'absence de noms de serveurs privés, identifiants, secrets et données clients. Ajouter uniquement de **vraies captures** de l'application. Ne pas publier `appsettings.Local.json`, bases SQL, exports de données sensibles ou binaires de compilation.

Licence : voir [LICENSE](LICENSE).
