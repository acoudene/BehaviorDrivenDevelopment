# language: fr
@AssetManagement @QrCode
Fonctionnalité: Interprétation d'un QR Code d'équipement
    En tant que technicien de maintenance en intervention sur le terrain
    Je veux scanner le QR Code apposé sur un équipement
    Afin d'identifier de façon fiable l'asset concerné avant toute déclaration

    Contexte:
        Étant donné le référentiel d'assets suivant
            | Identifiant                          | Tag         | Désignation                  | Localisation                  | Statut  |
            | 3f2504e0-4f89-41d3-9a0c-0305e82c3301 | CTA-A1-002  | Centrale Traitement d'Air    | Site Lyon - Bât. A - Toiture  | Actif   |
            | 7c9e6679-7425-40de-944b-e07fc1f90ae7 | ASC-B2-007  | Ascenseur passagers          | Site Lyon - Bât. B - Niveau 2 | Déposé  |

    Scénario: Scan d'un QR Code valide pointant vers un équipement actif
        Étant donné un QR Code encodant l'URL "https://gmao.exemple.fr/assets/3f2504e0-4f89-41d3-9a0c-0305e82c3301"
        Quand le technicien scanne le QR Code
        Alors le QR Code est résolu avec succès
        Et l'équipement identifié porte le tag "CTA-A1-002"
        Et l'équipement identifié est situé à "Site Lyon - Bât. A - Toiture"

    Scénario: Scan d'un contenu qui n'est pas une URL exploitable
        Étant donné un QR Code encodant l'URL "ceci-n-est-pas-une-url"
        Quand le technicien scanne le QR Code
        Alors l'interprétation échoue avec le motif "QR Code malformé"

    Scénario: Scan d'une URL valide vers un équipement absent du référentiel
        Étant donné un QR Code encodant l'URL "https://gmao.exemple.fr/assets/00000000-0000-0000-0000-000000000099"
        Quand le technicien scanne le QR Code
        Alors l'interprétation échoue avec le motif "Équipement inconnu"

    Scénario: Scan du QR Code d'un équipement déposé
        Étant donné un QR Code encodant l'URL "https://gmao.exemple.fr/assets/7c9e6679-7425-40de-944b-e07fc1f90ae7"
        Quand le technicien scanne le QR Code
        Alors l'interprétation échoue avec le motif "Équipement déposé"
        Et l'équipement identifié porte le tag "ASC-B2-007"

    Plan du scénario: Robustesse de l'extraction d'identifiant selon le format scanné
        Étant donné un QR Code encodant l'URL "<contenu>"
        Quand le technicien scanne le QR Code
        Alors l'interprétation aboutit au motif "<motif>"

        Exemples:
            | contenu                                                              | motif              |
            | https://gmao.exemple.fr/assets/3f2504e0-4f89-41d3-9a0c-0305e82c3301   | Résolu             |
            | https://gmao.exemple.fr/assets/                                      | QR Code malformé   |
            | https://gmao.exemple.fr/3f2504e0-4f89-41d3-9a0c-0305e82c3301          | QR Code malformé   |
            | ftp://gmao.exemple.fr/assets/3f2504e0-4f89-41d3-9a0c-0305e82c3301     | QR Code malformé   |
            |                                                                      | QR Code malformé   |
