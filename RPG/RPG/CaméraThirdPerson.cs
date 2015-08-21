using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
   public class CaméraThirdPerson : Caméra
   {
      const float ANGLE_MIN = MathHelper.Pi / 16;
      const float VITESSE_ROTATION = 1f;
      const float VITESSE_ZOOM = 15f;
      /// <summary>
      /// Distance minimale de la caméra par rapport à l'objet
      /// </summary>
      const float ZOOM_MIN = 10;
      /// <summary>
      /// Distance maximale de la caméra par rapport à l'objet
      /// </summary>
      const float ZOOM_MAX = 84; 
 
      Vector3 Direction { get; set; }
      Vector3 Latéral { get; set; }
      Vector3 Offset { get; set; }

      float Zoom { get; set; }
      float RotationX { get; set; }

      ObjetDeBasePhysique ObjetÀSuivre { get; set; }

      float TempsÉcouléDepuisMAJ { get; set; }
      InputManager3D InputMgr { get; set; }


      public CaméraThirdPerson(RPG jeu, InputManager3D inputMgr, Vector3 positionCaméra, ObjetDeBasePhysique objetÀSuivre, Vector3 orientation)
         : base(jeu)
      {
         InputMgr = inputMgr;
         ObjetÀSuivre = objetÀSuivre;
         CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
         CréerPointDeVue(positionCaméra, ObjetÀSuivre.Position, orientation);
      }

      public override void Initialize()
      {
         TempsÉcouléDepuisMAJ = 0;
         base.Initialize();
      }

      protected override void CréerPointDeVue()
      {
         Latéral              = Vector3.Cross(Direction, OrientationVerticale);
         Direction            = Vector3.Normalize(Direction);
         OrientationVerticale = Vector3.Normalize(OrientationVerticale);
         Latéral              = Vector3.Normalize(Latéral);
         Vue = Matrix.CreateLookAt(Position, ObjetÀSuivre.Position, OrientationVerticale);
         GénérerFrustum();
      }

      protected override void CréerPointDeVue(Vector3 position, Vector3 cible, Vector3 orientation)
      {
         Position = position;
         Cible = cible;
         OrientationVerticale = orientation;
         Direction = cible - position;
         RotationX = (float)Math.Atan2(-Direction.Y, Math.Sqrt(Math.Pow(Direction.X, 2) + Math.Pow(Direction.Z, 2)));
         Zoom = Direction.Length();
         CréerPointDeVue();
      }

      public override void Update(GameTime gameTime)
      {
         float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
         TempsÉcouléDepuisMAJ += TempsÉcoulé;
         if (TempsÉcouléDepuisMAJ >= Jeu.INTERVALLE_MAJ)
         {
            GérerRotation();
            GérerZoom();
            CréerPointDeVue();
            TempsÉcouléDepuisMAJ = 0;
         }
         base.Update(gameTime);
      }

      private int GérerTouche(Keys touche)
      {
         return InputMgr.ÉtatClavier.IsKeyDown(touche) ? 1 : 0;
      }

      private void GérerZoom()
      {
         float déplacementDirection = InputMgr.AncienÉtatSouris.ScrollWheelValue - InputMgr.ÉtatSouris.ScrollWheelValue;
         Zoom = MathHelper.Clamp(Zoom + déplacementDirection * VITESSE_ZOOM * TempsÉcouléDepuisMAJ, ZOOM_MIN, ZOOM_MAX);
         Direction = Vector3.Normalize(Direction);
         Direction *= Zoom;
         Position = ObjetÀSuivre.Position - Direction;
      }

      private void GérerRotation()
      {
         GérerLacet();
         GérerTangage();
      }

      private void GérerLacet()
      {
         float sensRotation = (GérerTouche(Keys.D) - GérerTouche(Keys.A));
         Matrix rotation = Matrix.CreateRotationY(VITESSE_ROTATION * sensRotation * TempsÉcouléDepuisMAJ);
         Direction = Vector3.Transform(Direction, rotation);
         OrientationVerticale = Vector3.Transform(OrientationVerticale, rotation);
      }

      private void GérerTangage()
      {
         float sensRotation = (GérerTouche(Keys.W) - GérerTouche(Keys.S));
         float deltaRotationX = VITESSE_ROTATION * sensRotation * TempsÉcouléDepuisMAJ;
         Matrix rotation;
         if (RotationX + deltaRotationX > MathHelper.PiOver2)
         {
            rotation = Matrix.CreateFromAxisAngle(Latéral, -(MathHelper.PiOver2 - RotationX));
            RotationX = MathHelper.PiOver2;
         }
         else
         {
            if (RotationX + deltaRotationX < 0)
            {
               rotation = Matrix.CreateFromAxisAngle(Latéral, RotationX);
               RotationX = 0;
            }
            else
            {
               rotation = Matrix.CreateFromAxisAngle(Latéral, -deltaRotationX);
               RotationX = RotationX + deltaRotationX;
            }
         }

         Direction = Vector3.Transform(Direction, rotation);
         OrientationVerticale = Vector3.Transform(OrientationVerticale, rotation);
      }

   }
}
