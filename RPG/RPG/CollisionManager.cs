using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class CollisionManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private RPG Jeu { get; set; }
        private ScèneDeJeu ScèneJeu { get; set; }

        public CollisionManager(RPG jeu, ScèneDeJeu scèneJeu)
            : base(jeu)
        {
            Jeu = jeu;
            ScèneJeu = scèneJeu;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public bool IsCollision(ObjetDeBasePhysique obj1, ObjetDeBasePhysique obj2)
        {
            bool collision = false;
            foreach (BoundingBox b1 in obj1.BoxList)
            {
                foreach (BoundingBox b2 in obj2.BoxList)
                {
                    if (b1.Intersects(b2))
                    {
                        collision = true;
                        break;
                    }
                }
            }
            return collision;
        }

        public void CheckCombattantHit(Combattant combattant)
        {
            foreach (ProjectileBalistique p in ScèneJeu.ProjManager.ListeProjectiles)
            {
               if (IsCollision(combattant, p) && !Combattant.Equals(combattant, p.Propriétaire) && (combattant is Monstre ^ p.Propriétaire is Monstre))
                {
                    combattant.PerdrePointsDeVie(p.Damage);
                    p.EstEnCollision = true;
                    break;
                }
            }
        }

        public bool CheckCollisionWithOtherPeople(ÊtreVivant etre, Vector3 oldPosition, Vector3 newPosition)
        {
            bool collision = false;

            List<BoundingBox> boxList = BuildBoundingBoxesWithOffset(oldPosition, newPosition, etre.BoxList);

            foreach (Monstre m in ScèneJeu.MonstManager.ListeMonstres)
            {
                if ((IsCollision(etre, m) && !Monstre.Equals(etre, m)) && (Vector2.Distance(m.PositionCoord, new Vector2(oldPosition.X, oldPosition.Z)) 
                                                                                > Vector2.Distance(m.PositionCoord, new Vector2(newPosition.X, newPosition.Z))))
                {
                    collision = true;
                    
                    break;
                }
            }

            return collision;
        }

        public bool CheckCollisionWithDoodads(ÊtreVivant etre, Vector3 oldPosition, Vector3 newPosition)
        {
           bool collision = false;
           List<BoundingBox> boxList = BuildBoundingBoxesWithOffset(oldPosition, newPosition, etre.BoxList);

           foreach (Doodad d in ScèneJeu.DoodManager.ListeDoodads)
           {
              Vector2 dpositioncoord = new Vector2(d.Position.X, d.Position.Z);
              if ((IsCollision(etre, d) && (Vector2.Distance(dpositioncoord, new Vector2(oldPosition.X, oldPosition.Z))
                                                                              > Vector2.Distance(dpositioncoord, new Vector2(newPosition.X, newPosition.Z))))
)              {
                 collision = true;

                 break;
              }
           }

           return collision;
        }
        // Creates a new list of bounding boxes with the original list and the old and new positions
        public List<BoundingBox> BuildBoundingBoxesWithOffset(Vector3 oldPosition, Vector3 newPosition, List<BoundingBox> originalBoxList)
        {
            Vector3 offset = new Vector3(newPosition.X - oldPosition.X, newPosition.Y - oldPosition.Y, newPosition.Z - oldPosition.Z);

            List<BoundingBox> newBoxList = new List<BoundingBox>();

            foreach (BoundingBox obox in originalBoxList)
            {
                Vector3 ptMin = obox.Min;
                Vector3 ptMax = obox.Max;

                Vector3 newMin = ptMin + offset;
                Vector3 newMax = ptMax + offset;

                newBoxList.Add(new BoundingBox(newMin, newMax));
            }

            return newBoxList;
        }

    }
}