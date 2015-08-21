using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AtelierXNA
{
   public class InputManager3D : InputManager
   {
      const float PRÉCISION_VÉRIFICATION = 0.0008f;
      ScèneDeJeu ScèneJeu { get; set; }

      public InputManager3D(RPG jeu, ScèneDeJeu scènejeu)
         : base(jeu)
      {
         ScèneJeu = scènejeu;
      }

      public Vector3 GetPositionSouris3d()
      {
         Vector3 nearScreenPoint = new Vector3(ÉtatSouris.X, ÉtatSouris.Y, 0);
         Vector3 farScreenPoint = new Vector3(ÉtatSouris.X, ÉtatSouris.Y, 1);
         Vector3 nearWorldPoint = Jeu.PériphériqueGraphique.GraphicsDevice.Viewport.Unproject(nearScreenPoint, ScèneJeu.CaméraJeu.Projection, ScèneJeu.CaméraJeu.Vue, Matrix.Identity);
         Vector3 farWorldPoint = Jeu.PériphériqueGraphique.GraphicsDevice.Viewport.Unproject(farScreenPoint, ScèneJeu.CaméraJeu.Projection, ScèneJeu.CaméraJeu.Vue, Matrix.Identity);
         Vector3 direction = farWorldPoint - nearWorldPoint;
         float xInitial;
         float zInitial;
         if (nearWorldPoint.Y > ScèneJeu.MapManager.HauteurMax)
         {
            xInitial = nearWorldPoint.X + ((ScèneJeu.MapManager.HauteurMax - nearWorldPoint.Y) / direction.Y) * direction.X;
            zInitial = nearWorldPoint.Z + ((ScèneJeu.MapManager.HauteurMax - nearWorldPoint.Y) / direction.Y) * direction.Z;
         }
         else
         {
            xInitial = nearWorldPoint.X;
            zInitial = nearWorldPoint.Z;
         }
         direction = direction * PRÉCISION_VÉRIFICATION;
         bool aTrouvéUnPoint = false;
         Vector3 positionClic = new Vector3(xInitial, Math.Min(ScèneJeu.MapManager.HauteurMax, nearWorldPoint.Y), zInitial);
         float hauteurPoint;
         while (!(aTrouvéUnPoint) && (ScèneJeu.CaméraJeu.Frustum.Contains(positionClic) == ContainmentType.Contains) && positionClic.Y <= ScèneJeu.MapManager.HauteurMax && positionClic.Y >= ScèneJeu.MapManager.HauteurMin)
         {
            positionClic = positionClic + direction;
            hauteurPoint = ScèneJeu.MapManager.CalculerHauteurPoint(new Vector2(positionClic.X, positionClic.Z));
            if (positionClic.Y <= hauteurPoint)
            {
               aTrouvéUnPoint = true;
            }
         }
         if (!aTrouvéUnPoint)
         {
            positionClic = Vector3.Zero;
         }
         return positionClic;
      }

      public Monstre GetSourisBoxIntercept(List<Monstre> monstres)
      {
         Vector3 nearScreenPoint = new Vector3(ÉtatSouris.X, ÉtatSouris.Y, 0);
         Vector3 farScreenPoint = new Vector3(ÉtatSouris.X, ÉtatSouris.Y, 1);
         Vector3 nearWorldPoint = Jeu.PériphériqueGraphique.GraphicsDevice.Viewport.Unproject(nearScreenPoint, ScèneJeu.CaméraJeu.Projection, ScèneJeu.CaméraJeu.Vue, Matrix.Identity);
         Vector3 farWorldPoint = Jeu.PériphériqueGraphique.GraphicsDevice.Viewport.Unproject(farScreenPoint, ScèneJeu.CaméraJeu.Projection, ScèneJeu.CaméraJeu.Vue, Matrix.Identity);
         Vector3 direction = farWorldPoint - nearWorldPoint;
         Ray raySouris = new Ray(nearWorldPoint, direction);

         float distancemin = float.MaxValue;
         float? distance;
         Monstre cible = null;
         foreach (Monstre monstre in monstres)
         {
            foreach (BoundingBox box in monstre.BoxList)
            {
               distance = raySouris.Intersects(box);
               if (distance != null && distance < distancemin)
               {
                  distancemin = (float)distance;
                  cible = monstre;
               }
            }
         }
         return cible;
      }
   }

}