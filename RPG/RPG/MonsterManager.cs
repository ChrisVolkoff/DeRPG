using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    public class MonsterManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public RPG Jeu { get; private set; }
        public Sc�neDeJeu Sc�neJeu { get; private set; }

        public List<Monstre> ListeMonstres { get; private set; }
        private int CptMonster { get; set; }

        public MonsterManager(RPG jeu, Sc�neDeJeu sc�neJeu)
            : base(jeu)
        {
            Jeu = jeu;
            Sc�neJeu = sc�neJeu;
        }

        public override void Initialize()
        {
            ListeMonstres = new List<Monstre>();
            CptMonster = 0;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            CheckForMonstersToRemove();
            UpdateMonsters(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawMonsters(gameTime);
            base.Draw(gameTime);
        }

        public Monstre Cr�erMonstre(H�ros joueur, string nomMod�le, float scale, float �chelleBox, Vector3 positionInitiale, Vector3 rotationInitiale, Vector3 rotationOffset,
                        string name, float vitesseD�placementInitiale, float vitesseRotationInitiale, bool peutBougerEnTournant, float ptsVie,
                        int ptsD�fense, int ptsAttaque, int deltaDamage, float attackSpeed, bool isRange, float range, float aggrorange, int niveau, int id)
        {
           Monstre newMonster = new Monstre(Jeu, Sc�neJeu, joueur, nomMod�le, scale, �chelleBox, positionInitiale, rotationInitiale, rotationOffset, name, vitesseD�placementInitiale, vitesseRotationInitiale, peutBougerEnTournant, ptsVie, ptsD�fense, ptsAttaque, deltaDamage, attackSpeed, isRange, range, aggrorange, niveau, id);
            ListeMonstres.Add(newMonster);
            newMonster.Initialize();
            return newMonster;
        }

        private void CheckForMonstersToRemove()
        {
           for (int i = ListeMonstres.Count - 1; i >= 0; --i)
           {
              if (ListeMonstres[i].ToRemove)
              {
                 ListeMonstres[i].BoxList.Clear();
                 ListeMonstres[i].BoxDrawList.Clear();
                 ListeMonstres.RemoveAt(i);
              }
           }
        }

        private void UpdateMonsters(GameTime gameTime)
        {
            foreach (Monstre monster in ListeMonstres)
            {
               if (!(monster is Boss) && Vector3.Distance(monster.Position, Sc�neJeu.BaldorLeBrave.Position) <= Cam�ra.DISTANCE_PLAN_�LOIGN�)
                {
                    monster.Update(gameTime);
                }
            }
        }

        private void DrawMonsters(GameTime gameTime)
        {
            foreach (Monstre monster in ListeMonstres)
            {
                if (!(monster is Boss) && Vector3.Distance(monster.Position, Sc�neJeu.BaldorLeBrave.Position) <= Cam�ra.DISTANCE_PLAN_�LOIGN�)

                {
                    monster.Draw(gameTime);
                }
            }
        }

        // Get an ID for a new Monster
        public int GetID()
        {
            ++CptMonster;
            return (CptMonster - 1);
        }

        // Checks if a monster is active using his ID
        public bool IsIDActive(int ID)
        {
            bool result = ListeMonstres.Exists(delegate(Monstre monst)
            {
                return monst.ID == ID;
            }
            );
            return result;
        }

        public Monstre GetMonsterWithID(int id)
        {
            return ListeMonstres.Find(delegate(Monstre monst)
            {
                return monst.ID == id;
            }
            );
        }
    }
}