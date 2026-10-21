# Améliorations de cette édition

- Création d'une structure de dépôt claire `backend/`, `frontend/`, `docs/`.
- Consolidation des variantes backend en conservant les mappings SSAS de `work_back_v5`.
- Suppression des binaires/objets et dépendances embarquées ainsi que des README de correctifs temporaires.
- Connexion SSAS locale externalisée vers un fichier ignoré par Git.
- Exécution libre MDX désactivée par défaut.
- API : erreurs MDX propagées, journalisation des mesures en échec et erreurs HTTP génériques sans détails sensibles.
- Validation explicite des filtres MDX incorrects au lieu de les ignorer.
- Dimensions : borne Top N appliquée réellement aux membres renvoyés.
- Angular : proxy activé par défaut, correction du rendu des séries à zéro/négatives et affichage des pannes partielles du dashboard.
- Tables : `null` n'est plus affiché comme un `0` inventé ; Dimensions nettoie les résultats périmés lors d'erreurs.
- Suppression du fallback incorrect qui faisait passer des pays pour des villes.
- Documentation d'installation et de vérification, sans prétendre inclure un ETL ou un cube non fournis.

**Non inclus :** nouveau Data Warehouse, nouveaux packages SSIS, nouveau cube SSAS, fausses données de démo, authentification utilisateur, déploiement serveur ou tests de bout en bout sur SSAS.
