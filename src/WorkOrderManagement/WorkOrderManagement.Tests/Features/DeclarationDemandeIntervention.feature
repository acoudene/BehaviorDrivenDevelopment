# language: fr
@WorkOrderManagement @DemandeIntervention
Fonctionnalité: Déclaration d'une demande d'intervention sur un équipement
    En tant que technicien ayant scanné le QR Code d'un équipement
    Je veux déclarer une demande d'intervention pour signaler une panne
    Afin que la maintenance soit planifiée et tracée

    Contexte:
        Étant donné le catalogue d'assets exposé par AssetManagement
            | Identifiant                          | Tag         | Désignation               | Localisation                  | Statut  |
            | 3f2504e0-4f89-41d3-9a0c-0305e82c3301 | CTA-A1-002  | Centrale Traitement d'Air | Site Lyon - Bât. A - Toiture  | Actif   |
            | 7c9e6679-7425-40de-944b-e07fc1f90ae7 | ASC-B2-007  | Ascenseur passagers       | Site Lyon - Bât. B - Niveau 2 | Déposé  |

    Scénario: Déclaration réussie d'une panne sur un équipement actif
        Étant donné l'équipement résolu d'identifiant "3f2504e0-4f89-41d3-9a0c-0305e82c3301"
        Quand le technicien "j.martin" déclare une panne "Arrêt complet de la CTA, plus de soufflage" en priorité "Critique"
        Alors une demande d'intervention est créée
        Et la demande référence l'équipement "CTA-A1-002 - Centrale Traitement d'Air"
        Et la demande est en priorité "Critique"
        Et la demande est déclarée par "j.martin"
        Et la demande est au statut "Soumise"

    Scénario: Refus de déclaration sur un équipement déposé
        Étant donné l'équipement résolu d'identifiant "7c9e6679-7425-40de-944b-e07fc1f90ae7"
        Quand le technicien "j.martin" déclare une panne "Porte bloquée" en priorité "Normale"
        Alors aucune demande d'intervention n'est créée
        Et la déclaration est refusée avec le motif "Équipement non éligible"

    Scénario: Refus de déclaration pour un équipement inconnu du catalogue
        Étant donné l'équipement résolu d'identifiant "00000000-0000-0000-0000-000000000099"
        Quand le technicien "j.martin" déclare une panne "Bruit anormal" en priorité "Normale"
        Alors aucune demande d'intervention n'est créée
        Et la déclaration est refusée avec le motif "Équipement non éligible"

    Scénario: Refus de déclaration sans description de panne
        Étant donné l'équipement résolu d'identifiant "3f2504e0-4f89-41d3-9a0c-0305e82c3301"
        Quand le technicien "j.martin" déclare une panne "" en priorité "Normale"
        Alors aucune demande d'intervention n'est créée
        Et la déclaration est refusée avec le motif "Signalement invalide"
