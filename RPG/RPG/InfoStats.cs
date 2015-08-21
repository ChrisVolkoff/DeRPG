using Microsoft.Xna.Framework;

namespace AtelierXNA
{
   public class InfoStats : BoutonInfo
   {
      Héros HérosASuivre { get; set; }

      public InfoStats(RPG jeu, ScèneDeJeu scenejeu, Héros hérosASuivre, Rectangle emplacement, string nomfont, string nomtextBackground)
         : base(jeu, scenejeu, emplacement, null, null, nomfont, nomtextBackground, null, null)
      {
         HérosASuivre = hérosASuivre;
      }

      public override void Update(GameTime gameTime)
      {
         Text = HérosASuivre.Name +
         "\n" + "Vie: " + HérosASuivre.PtsVie.ToString() + "/" + HérosASuivre.PtsVieMax.ToString() +
         "\n" + "Mana: " + HérosASuivre.PtsRessource.ToString() + "/" + HérosASuivre.PtsRessourceMax.ToString() +
         "\n" + "Niveau: " + HérosASuivre.Niveau.ToString() +
         "\n" + "Expérience: " + HérosASuivre.PtsExp.ToString() + "/" + HérosASuivre.ExpProchainNiveau.ToString() +
         "\n" + "Attaque: " + HérosASuivre.PtsAttaque.ToString() + " - " + (HérosASuivre.PtsAttaque + HérosASuivre.DeltaDamage).ToString() +
         "\n" + "Défense: " + HérosASuivre.PtsDéfense.ToString() +
         "\n" + "Vitesse de frappe: " + (1 / HérosASuivre.AttackSpeed).ToString() + " attaque(s) par seconde";

         base.Update(gameTime);
      }
   }
}
