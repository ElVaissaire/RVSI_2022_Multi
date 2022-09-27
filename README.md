
Martine NOGUES
Eléonore VAISSAIRE

Bataille de boules de neiges

Commandes :
    -avancer            z
    -aller à gauche     q
    -aller à droite     d
    -aller en arrière   s
    -lancer une boule   espace

    -orienter la vision -> bouger la souris

Chaque joueur a 5 vies
Jeu pour le moment en local
Jusqu'à 8 joueurs


NOTE : Le host peut rejouer en cliquant sur "Play Again", mais pas le client.
Le client est bien téléporté loin, en revanche, il ne peut plus revenir en 0,0,0
(le tableau m_playerArray[] du GameManager reste à null si on est client
Donc le client n'appelle jamais la fonction Respawn())