using System;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{

    public class Combattant : �treVivant
    {
        float TempsDepuisMAJ { get; set; }
        float TempsDepuisAttaque { get; set; }

        public float AttackSpeed { get; protected set; }
        public float Range { get; private set; }
        public float PtsVie { get; protected set; }
        public float PtsVieMax { get; protected set; }
        public int PtsAttaque { get; protected set; }
        public int PtsD�fense { get; protected set; }
        public int DeltaDamage { get; protected set; }

        public bool IsRange { get; private set; }
        protected Combattant Cible { get; set; }
        public bool ToRemove { get { return PtsVie <= 0; } }


        public Combattant(RPG game, Sc�neDeJeu sc�neJeu, String nomMod�le, float scale, float �chelleBox, Vector3 positionInitiale, Vector3 rotationInitiale, Vector3 rotationOffset,
                          string name, float vitesseD�placementInitiale, float vitesseRotationInitiale, bool peutBougerEnTournant, float ptsVie,
                          int ptsD�fense, int ptsAttaque, int deltaDamage, float attackSpeed, bool isRange, float range)
           : base(game, sc�neJeu, nomMod�le, scale, �chelleBox,positionInitiale, rotationInitiale, rotationOffset, name, vitesseD�placementInitiale, vitesseRotationInitiale,
                   peutBougerEnTournant)
        {
            PtsVie = ptsVie;
            PtsVieMax = ptsVie;
            PtsD�fense = PtsD�fense;
            PtsAttaque = ptsAttaque;
            DeltaDamage = deltaDamage;
            AttackSpeed = attackSpeed;
            Range = range;
            IsRange = isRange;
        }

        public override void Initialize()
        {
            TempsDepuisMAJ = 0;
  
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
           if (TempsDepuisMAJ >= Jeu.INTERVALLE_MAJ)
            {

                TempsDepuisAttaque += TempsDepuisMAJ;
                if (Cible != null && ActionActuelle == Action.ATTAQUER)
                {
                    if (Vector3.Distance(Cible.Position, Position) <= Range)
                    {
                        if (TempsDepuisAttaque > AttackSpeed)
                        {
                            if (!IsRange)
                            {
                                AttaquerMelee();
                            }
                            else
                            {
                                AttaquerRanged();
                            }
                            TempsDepuisAttaque = 0;
                        }

                    }
                    else
                    {
                        Vector3 direction = Vector3.Normalize(Cible.Position - Position);
                        D�buterD�placement(Cible.Position - direction * (Range-0.01f));
                    }
                }
                TempsDepuisMAJ = 0;
            }
            Sc�neJeu.CollisionManager.CheckCombattantHit(this);
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsDepuisMAJ += temps�coul�;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected virtual void AttaquerMelee()
        {
            int D�gats = PtsAttaque + Jeu.G�n�rateurAl�atoire.Next(0, DeltaDamage + 1);
            Cible.PerdrePointsDeVie(D�gats);
            if (Cible.PtsVie <= 0)
            {
                Cible = null;
                PositionCible = PositionCoord;

            }
            ActionActuelle = Action.ATTENDRE;
        }

        protected virtual void AttaquerRanged()
        {

            int D�gats = Math.Max((PtsAttaque + Jeu.G�n�rateurAl�atoire.Next(0, DeltaDamage + 1)) - Cible.PtsD�fense, 0);
            Sc�neJeu.ProjManager.Cr�erProjectile(this, Cible.Position, D�gats, Range);
            if (Cible.PtsVie <= 0)
            {
                Cible = null;
                PositionCible = PositionCoord;
                ActionActuelle = Action.ATTENDRE;
            }
        }
        public virtual void PerdrePointsDeVie(int D�gats)
        {
            PtsVie = PtsVie - Math.Max((D�gats-PtsD�fense), 0);
        }

        public virtual void D�buterAttaque(Combattant cible)
        {
            ActionActuelle = Action.ATTAQUER;
            Cible = cible;
            PositionCible = PositionCoord;
        }
    }
}
