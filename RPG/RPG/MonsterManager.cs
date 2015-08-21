using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    public class MonsterManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public RPG Jeu { get; private set; }
        public ScèneDeJeu ScèneJeu { get; private set; }

        public List<Monstre> ListeMonstres { get; private set; }
        private int CptMonster { get; set; }

        public MonsterManager(RPG jeu, ScèneDeJeu scèneJeu)
            : base(jeu)
        {
            Jeu = jeu;
            ScèneJeu = scèneJeu;
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

        public Monstre CréerMonstre(Héros joueur, string nomModèle, float scale, float échelleBox, Vector3 positionInitiale, Vector3 rotationInitiale, Vector3 rotationOffset,
                        string name, float vitesseDéplacementInitiale, float vitesseRotationInitiale, bool peutBougerEnTournant, float ptsVie,
                        int ptsDéfense, int ptsAttaque, int deltaDamage, float attackSpeed, bool isRange, float range, float aggrorange, int niveau, int id)
        {
           Monstre newMonster = new Monstre(Jeu, ScèneJeu, joueur, nomModèle, scale, échelleBox, positionInitiale, rotationInitiale, rotationOffset, name, vitesseDéplacementInitiale, vitesseRotationInitiale, peutBougerEnTournant, ptsVie, ptsDéfense, ptsAttaque, deltaDamage, attackSpeed, isRange, range, aggrorange, niveau, id);
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
               if (!(monster is Boss) && Vector3.Distance(monster.Position, ScèneJeu.BaldorLeBrave.Position) <= Caméra.DISTANCE_PLAN_ÉLOIGNÉ)
                {
                    monster.Update(gameTime);
                }
            }
        }

        private void DrawMonsters(GameTime gameTime)
        {
            foreach (Monstre monster in ListeMonstres)
            {
                if (!(monster is Boss) && Vector3.Distance(monster.Position, ScèneJeu.BaldorLeBrave.Position) <= Caméra.DISTANCE_PLAN_ÉLOIGNÉ)

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