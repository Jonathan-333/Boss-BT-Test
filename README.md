# Boss BT Test

Un petit jeu Unity en vue de dessus où les ennemis et le boss sont contrôlés par un **behaviour tree** écrit à la main en C#, sans plugin. Fait à deux au cégep, sur Unity 6.

<!-- Capture d'écran ou GIF du jeu : à ajouter ici (ex. docs/gameplay.gif) -->

## Le behaviour tree

Le moteur est dans `Assets/Scripts/BehaviourTree/Base Classes` :

- **`BehaviourNode`** : un nœud avec ses conditions, qui s'exécute, se termine (succès ou échec) et peut être interrompu.
- **`BehaviourComposite`** : trois types de composites, **Selector**, **Sequence** et **TresholdSequence** (exécute toute la séquence et réussit si assez d'enfants ont réussi).
- **`BehaviourCondition`** : une condition évaluée avant de lancer un nœud.
- **`BehaviourInterupt`** : surveille des conditions en continu et interrompt les nœuds actifs quand l'une d'elles change (par exemple, le joueur entre dans la zone de poursuite pendant une patrouille).
- **`BehaviourTree`** : le `MonoBehaviour` qui construit l'arbre et fait avancer le nœud actif à chaque frame.

### Les ennemis (`Enemy_BT`)

```
Selector
├── Sequence
│   ├── Poursuite   (si le joueur est assez proche)
│   └── Repos       (après une poursuite trop longue)
└── Patrouille
```

Une interruption relance l'arbre dès que le joueur entre ou sort de la zone de poursuite.

### Le boss (`Boss_BT`)

```
Selector
├── Faire apparaître des ennemis   (quand le temps de recharge est écoulé)
├── Sequence
│   ├── Poursuite
│   ├── Faire apparaître des ennemis
│   ├── Commencer à tirer
│   ├── Tourner au centre de la carte
│   └── Arrêter de tirer
└── Patrouille
```

## Le jeu

- **Joueur** : déplacement, dash (avec un dash de précision à la souris), points de vie et mana.
- **Combat** : projectiles, dégâts au contact et invincibilité temporaire après un coup.
- **Effets** : secousse de caméra, particules, post-processing et ralenti.
- **Événements** : un système d'événements basé sur des `ScriptableObject` (inspiré de la vidéo [*ULTIMATE Event System for Unity Beginners*](https://www.youtube.com/watch?v=7_dyDmF0Ktw) de This Is GameDev).

## Contrôles

| Action | Clavier / souris | Manette |
|---|---|---|
| Se déplacer | WASD ou flèches | Joystick gauche |
| Dash | Espace | A |
| Dash de précision | Clic gauche (vise la souris) | Gâchette droite (RT), visée au joystick droit |

## Lancer le projet

1. Ouvrir le dossier avec **Unity 6000.2.7f2** (ou une version 6000.2 plus récente).
2. Ouvrir la scène `Assets/Scenes/MainGame.unity` et appuyer sur Play.

Technique : Unity 6, URP, Input System, C#.

## Équipe

[@Jonathan-333](https://github.com/Jonathan-333) et [@OnlineAquaCow](https://github.com/OnlineAquaCow).
