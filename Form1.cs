using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JDR
{
    public partial class Assisstant_JDR : Form
    {
        public struct ObjetInv 
        {
            public string Nom, Type, Rarete, BonusType, EffetType1, EffetDes1, EffetType2, EffetDes2, EffetType3, EffetDes3, Portee;
            public int Bonus, Prix, Quantité;
            public bool EmplOccupe;
        }

        public struct Competence
        {
            public string Nom, Type, Effet;
            public int Nb_tours, CD, BonusPV, BonusPre, BonusEsq, BonusCrit, BonusVit, BonusCons, BonusInt, BonusCha, BonusSau, BonusIns;
        }

        public struct Classe
        {
            //Remplis avec la fonction initClasse

            public string NomClasse;
            public string Equipable1, Equipable2, Equipable3, Equipable4, Equipable5, Equipable6;
            public int ExpAct, ExpSup, Niv;

            public int PreBase, EsqBase, CritBase;
            public double PVBase, VitBase, ConsBase, IntBase, ChaBase, SauBase, InsBase;
            public string MvtBase, Dégâts;

            public double PVGrowth, VitGrowth, ConsGrowth, IntGrowth, ChaGrowth, SauGrowth, InsGrowth;

            public int PVEquip, PreEquip, EsqEquip, CritEquip;
            public int VitEquip, ConsEquip, IntEquip, ChaEquip, SauEquip, InsEquip;

            public int MvtBuff, PreBuff, EsqBuff, CritBuff;
            public int VitBuff, ConsBuff, IntBuff, ChaBuff, SauBuff, InsBuff;

            public int PreTot, EsqTot, CritTot;
            public double PVTotAct, PVTotMax, VitTot, ConsTot, IntTot, ChaTot, SauTot, InsTot;
            public string MvtTot;

            //Remplis avec le Panel Creation Fiche Social

            public int SocialBase, SocialTot;
        }

        //Remplis avec les Panels Création Fiche Nom, Talent, Statut et Age
        public string Nom, Talent, Statut, Age;
        public int Fonds, PtsComp, PtsCompApres;
        public int BonusSoc, BonusPV, BonusPre, BonusEsq, BonusCrit, BonusVit, BonusCons, BonusInt, BonusCha, BonusSau, BonusIns;
        public int Chapitre;
        public string Phase, MomentJournée;
        public int Date;

        //Remplis avec la fonction initClasse et le Panel Creation Fiche Achat
        const int nb_objets_inv_joueur = 8;
        public ObjetInv[] InventaireJoueur = new ObjetInv[nb_objets_inv_joueur];

        public bool flag_marche_creation_perso = false, flag_achat_conso_double;
        public string marché_achat_type, marché_achat_rarete;
        public double MultiplicateurArgent = 1;
        public bool flag_comp_alchimie = false;
        public int tour_combat;

        public Classe ClassePerso;

        byte[] Ecriture_fiche_perso;

        const int nb_objets_inv = 450;
        const int nb_objets_marché = 6;
        const int nb_comp = 200;
        const int nb_comp_joueur = 8;
        public ObjetInv[] Inventaire = new ObjetInv[nb_objets_inv];
        public ObjetInv[] ObjetsMarché = new ObjetInv[nb_objets_marché];
        public ObjetInv[] ObjetsAlchimie = new ObjetInv[nb_objets_marché];
        public Competence[] Competences = new Competence[nb_comp];
        public Competence[] CompJoueur = new Competence[nb_comp_joueur];
        public int[] ConsommablesDoubles = new int[nb_objets_inv_joueur];
        public int nb_consos_meme_nom;

        public Competence comp_selectionee, new_comp;
        public bool comp_marche = false;

        public string comp_alchimie_obj1 = "", comp_alchimie_obj2 = "", comp_alchimie_rarete, comp_alchimie_type;
        public ObjetInv comp_alchimie_chx_objet;
        public int comp_alchimie_qte_tot, comp_alchimie_qte_1, comp_alchimie_qte_2;

        public int ptPV, ptPre, ptEsq, ptCrit, ptVit, ptCons, ptInt, ptCha, ptSau, ptIns;

        public int nb_pts_custom, nb_pts_custom_apres;

        FileStream Doc_fiche_perso_ecriture;
        string[] LignesFichierSauvegarde;
        public double tempconversion;
        public string chx_activite = "Rien";

        public Assisstant_JDR()
        {
            InitializeComponent();
        }

        //Bouton Création Fiche
        private void BtnNewFiche_Click(object sender, EventArgs e)
        {
            PnlCreationFicheNom.Location = new Point(0, 0);
            PnlCreationFicheNom.Visible = true;

            File.WriteAllText("fiche_perso_stats.txt", string.Empty);
            Chapitre = 1;
            Phase = "d'enquete";
            tour_combat = 1;
            Date = 1;
            MomentJournée = "Matin";
            ClassePerso.Niv = 1;
            initInventaire(ref Inventaire);
            initComp(ref Competences);
            
            flag_marche_creation_perso = true;
        }

        //Panel Création Fiche Nom
        private void BtnCreationFicheNomRetour_Click(object sender, EventArgs e)
        {
            PnlCreationFicheNom.Visible = false;
        }

        private void BtnCreationFicheNomSuivant_Click(object sender, EventArgs e)
        {
            if(TxtBoxCreationFicheNom.Text != "")
            {
                Nom = TxtBoxCreationFicheNom.Text;

                PnlCreationFicheNom.Visible = false;
                PnlCreationFicheTalent.Location = new Point(0, 0);
                PnlCreationFicheTalent.Visible = true;
            }
        }

        //Panel Création Fiche Talent
        private void BtnCreationFicheTalentTravailleur_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheTalent.Text = "+1 point de compétence par niveau";
            Talent = "Travailleur";
        }

        private void BtnCreationFicheTalentChanceux_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheTalent.Text = "Argent gagné +50%";
            Talent = "Chanceux";
        }

        private void BtnCreationFicheTalentSoigneux_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheTalent.Text = "Bonus conférés par les armes +10%";
            Talent = "Soigneux";
        }

        private void BtnCreationFicheTalentChiant_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheTalent.Text = "Inverse les bonus/malus reçus";
            Talent = "Chiant";
        }

        private void BtnCreationFicheTalentDebrouillard_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheTalent.Text = "Augmente de 10% le taux d'activation des compétences";
            Talent = "Debrouillard";
        }

        private void BtnCreationFicheTalentCharismatique_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheTalent.Text = "Peut relancer un dé une fois par période";
            Talent = "Charismatique";
        }

        private void BtnCreationFicheTalentBizarre_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheTalent.Text = "Inverse le nombre de points de compétences requis pour augmenter les stats";
            Talent = "Bizarre";
        }

        private void BtnCreationFicheTalentResilient_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheTalent.Text = "Réanime le personnage une fois par combat en cas de mort";
            Talent = "Resilient";
        }

        private void BtnCreationFicheTalentRetour_Click(object sender, EventArgs e)
        {
            PnlCreationFicheTalent.Visible = false;
            PnlCreationFicheNom.Visible = true;
        }

        private void BtnCreationFicheTalentSuivant_Click(object sender, EventArgs e)
        {
            if((Talent == "Travailleur") || (Talent == "Chanceux") || (Talent == "Soigneux") || (Talent == "Chiant") || (Talent == "Debrouillard") || (Talent == "Charismatique") || (Talent == "Bizarre") || (Talent == "Resilient"))
            {
                PnlCreationFicheTalent.Visible = false;
                PnlCreationFicheStatut.Location = new Point(0, 0);
                PnlCreationFicheStatut.Visible = true;
            }
        }

        //Panel Creation Fiche Statut
        private void BtnCreationFicheStatutOuvrier_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheStatut.Text = "Vous commencez avec 5 pts de compétence, 200G et +3 en Social";
            Statut = "Classe ouvriere";
            PtsComp = 5;
            Fonds = 200;
            BonusSoc = 3;
        }

        private void BtnCreationFicheStatutMoyen_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheStatut.Text = "Vous commencez avec 3 pts de compétence, 500G et +5 en Social";
            Statut = "Classe moyenne";
            PtsComp = 3;
            Fonds = 500;
            BonusSoc = 5;
        }

        private void BtnCreationFicheStatutSuperieur_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheStatut.Text = "Vous commencez avec 1 pt de compétence, 1500G et +0 en Social";
            Statut = "Classe superieure";
            PtsComp = 1;
            Fonds = 1500;
            BonusSoc = 0;
        }

        private void BtnCreationFicheStatutRetour_Click(object sender, EventArgs e)
        {
            PnlCreationFicheStatut.Visible = false;
            PnlCreationFicheTalent.Visible = true;
        }

        private void BtnCreationFicheStatutSuivant_Click(object sender, EventArgs e)
        {
            if ((Statut == "Classe ouvriere") || (Statut == "Classe moyenne") || (Statut == "Classe superieure"))
            {
                PnlCreationFicheStatut.Visible = false;
                PnlCreationFicheAge.Location = new Point(0, 0);
                PnlCreationFicheAge.Visible = true;
            }
        }

        //Panel Création Fiche Age
        private void BtnCreationFicheAgeAdulte_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheAge.Text = "PV/Pré/Vit/Int/Sau + 3";
            Age = "Adulte";
            BonusPV = 3;
            BonusPre = 3;
            BonusEsq = 0;
            BonusCrit = 0;
            BonusVit = 3;
            BonusCons = 0;
            BonusInt = 3;
            BonusCha = 0;
            BonusSau = 3;
            BonusIns = 0;
        }

        private void BtnCreationFicheAgeEnfant_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheAge.Text = "Crit/Esq/Cons/Cha/Ins + 3";
            Age = "Enfant";
            BonusPV = 0;
            BonusPre = 0;
            BonusEsq = 3;
            BonusCrit = 3;
            BonusVit = 0;
            BonusCons = 3;
            BonusInt = 0;
            BonusCha = 3;
            BonusSau = 0;
            BonusIns = 3;
        }

        private void BtnCreationFicheAgeRetour_Click(object sender, EventArgs e)
        {
            PnlCreationFicheAge.Visible = false;
            PnlCreationFicheStatut.Visible = true;
        }

        private void BtnCreationFicheAgeSuivant_Click(object sender, EventArgs e)
        {
            if((Age == "Enfant") || (Age == "Adulte"))
            {
                PnlCreationFicheAge.Visible = false;
                PnlCreationFicheClasse.Location = new Point(0, 0);
                PnlCreationFicheClasse.Visible = true;
            }
        }

        //Panel Creation Fiche Classe

        private void BtnCreationFicheClasseVagabond_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheClassePV.Text = "PV : Très bons";
            TxtBoxCreationFicheClassePV.BackColor = Color.Green;
            TxtBoxCreationFicheClasseDegats.Text = "Dégâts : 1D6";
            TxtBoxCreationFicheClasseDegats.BackColor = Color.Green;
            TxtBoxCreationFicheClasseSoc.Text = "Social : Bon";
            TxtBoxCreationFicheClasseSoc.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseMvt.Text = "Distance de mouvement : Bonne";
            TxtBoxCreationFicheClasseMvt.BackColor = Color.Yellow;
            TxtBoxCreationFicheClassePre.Text = "Précision : Très bonne";
            TxtBoxCreationFicheClassePre.BackColor = Color.Green;
            TxtBoxCreationFicheClasseEsq.Text = "Esquive : Moyenne";
            TxtBoxCreationFicheClasseEsq.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseCrit.Text = "Taux de critique : Mauvais";
            TxtBoxCreationFicheClasseCrit.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseVit.Text = "Vitesse : Excellente";
            TxtBoxCreationFicheClasseVit.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseCons.Text = "Constitution : Inégalée";
            TxtBoxCreationFicheClasseCons.BackColor = Color.Red;
            TxtBoxCreationFicheClasseInt.Text = "Intelligence : Abyssale";
            TxtBoxCreationFicheClasseInt.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseCha.Text = "Chance : Moyenne";
            TxtBoxCreationFicheClasseCha.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseSau.Text = "Sauvagerie : Bonne";
            TxtBoxCreationFicheClasseSau.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseIns.Text = "Instinct : Bon";
            TxtBoxCreationFicheClasseIns.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseDescription1.Text = "Offensif et résistant, le vagabond est le meilleur choix pour équilibrer attaque au CàC et défense.";
            TxtBoxCreationFicheClasseDescription2.Text = "";
            ClassePerso.NomClasse = "Vagabond";
        }

        private void BtnCreationFicheClasseCombattant_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheClassePV.Text = "PV : Bons";
            TxtBoxCreationFicheClassePV.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseDegats.Text = "Dégâts : 1D8";
            TxtBoxCreationFicheClasseDegats.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseSoc.Text = "Social : Moyen";
            TxtBoxCreationFicheClasseSoc.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseMvt.Text = "Distance de mouvement : Bonne";
            TxtBoxCreationFicheClasseMvt.BackColor = Color.Yellow;
            TxtBoxCreationFicheClassePre.Text = "Précision : Moyenne";
            TxtBoxCreationFicheClassePre.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseEsq.Text = "Esquive : Moyenne";
            TxtBoxCreationFicheClasseEsq.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseCrit.Text = "Taux de critique : Bon";
            TxtBoxCreationFicheClasseCrit.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseVit.Text = "Vitesse : Abyssale";
            TxtBoxCreationFicheClasseVit.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseCons.Text = "Constitution : Très bonne";
            TxtBoxCreationFicheClasseCons.BackColor = Color.Green;
            TxtBoxCreationFicheClasseInt.Text = "Intelligence : Très bonne";
            TxtBoxCreationFicheClasseInt.BackColor = Color.Green;
            TxtBoxCreationFicheClasseCha.Text = "Chance : Mauvaise";
            TxtBoxCreationFicheClasseCha.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseSau.Text = "Sauvagerie : Excellente";
            TxtBoxCreationFicheClasseSau.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseIns.Text = "Instinct : Inégalé";
            TxtBoxCreationFicheClasseIns.BackColor = Color.Red;
            TxtBoxCreationFicheClasseDescription1.Text = "Un guerrier versatile entre CàC et distance qui améliore son Crit en éliminant un ennemi.";
            TxtBoxCreationFicheClasseDescription2.Text = "Capable de se spécialiser en s'orientant vers de gros dégâts ou vers du support en évoluant.";
            ClassePerso.NomClasse = "Combattant";
        }

        private void BtnCreationFicheClasseArcaniste_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheClassePV.Text = "PV : Mauvais";
            TxtBoxCreationFicheClassePV.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseDegats.Text = "Dégâts : 1D6";
            TxtBoxCreationFicheClasseDegats.BackColor = Color.Green;
            TxtBoxCreationFicheClasseSoc.Text = "Social : Mauvais";
            TxtBoxCreationFicheClasseSoc.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseMvt.Text = "Distance de mouvement : Mauvaise";
            TxtBoxCreationFicheClasseMvt.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClassePre.Text = "Précision : Bonne";
            TxtBoxCreationFicheClassePre.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseEsq.Text = "Esquive : Bonne";
            TxtBoxCreationFicheClasseEsq.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseCrit.Text = "Taux de critique : Très bon";
            TxtBoxCreationFicheClasseCrit.BackColor = Color.Green;
            TxtBoxCreationFicheClasseVit.Text = "Vitesse : Moyenne";
            TxtBoxCreationFicheClasseVit.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseCons.Text = "Constitution : Abyssale";
            TxtBoxCreationFicheClasseCons.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseInt.Text = "Intelligence : Inégalée";
            TxtBoxCreationFicheClasseInt.BackColor = Color.Red;
            TxtBoxCreationFicheClasseCha.Text = "Chance : Excellente";
            TxtBoxCreationFicheClasseCha.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseSau.Text = "Sauvagerie : Très bonne";
            TxtBoxCreationFicheClasseSau.BackColor = Color.Green;
            TxtBoxCreationFicheClasseIns.Text = "Instinct : Moyen";
            TxtBoxCreationFicheClasseIns.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseDescription1.Text = "Un guerrier fragile mais capable d'infliger des gros dégâts à distance et dans une moindre mesure au CàC.";
            TxtBoxCreationFicheClasseDescription2.Text = "Seul détenteur d'une riposte, il peut aussi OS les ennemis en évoluant.";
            ClassePerso.NomClasse = "Arcaniste";
        }

        private void BtnCreationFicheClasseServiteur_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheClassePV.Text = "PV : Bons";
            TxtBoxCreationFicheClassePV.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseDegats.Text = "Dégâts : 1D6";
            TxtBoxCreationFicheClasseDegats.BackColor = Color.Green;
            TxtBoxCreationFicheClasseSoc.Text = "Social : Moyen";
            TxtBoxCreationFicheClasseSoc.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseMvt.Text = "Distance de mouvement : Bonne";
            TxtBoxCreationFicheClasseMvt.BackColor = Color.Yellow;
            TxtBoxCreationFicheClassePre.Text = "Précision : Excellente";
            TxtBoxCreationFicheClassePre.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseEsq.Text = "Esquive : Très bonne";
            TxtBoxCreationFicheClasseEsq.BackColor = Color.Green;
            TxtBoxCreationFicheClasseCrit.Text = "Taux de critique : Très bon";
            TxtBoxCreationFicheClasseCrit.BackColor = Color.Green;
            TxtBoxCreationFicheClasseVit.Text = "Vitesse : Inégalée";
            TxtBoxCreationFicheClasseVit.BackColor = Color.Red;
            TxtBoxCreationFicheClasseCons.Text = "Constitution : Bonne";
            TxtBoxCreationFicheClasseCons.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseInt.Text = "Intelligence : Moyenne";
            TxtBoxCreationFicheClasseInt.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseCha.Text = "Chance : Abyssale";
            TxtBoxCreationFicheClasseCha.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseSau.Text = "Sauvagerie : Moyenne";
            TxtBoxCreationFicheClasseSau.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseIns.Text = "Instinct : Mauvais";
            TxtBoxCreationFicheClasseIns.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseDescription1.Text = "Un DPS qui sacrifie ses défenses au prix d'une grande précision et d'un grand potentiel offensif au CàC.";
            TxtBoxCreationFicheClasseDescription2.Text = "Il corrige ce défaut en pouvant se mettre en duo avec un allié et est capable de gagner beaucoup d'argent.";
            ClassePerso.NomClasse = "Serviteur";
        }

        private void BtnCreationFicheClasseInfirmier_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheClassePV.Text = "PV : Abyssaux";
            TxtBoxCreationFicheClassePV.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseDegats.Text = "Dégâts : 1D6";
            TxtBoxCreationFicheClasseDegats.BackColor = Color.Green;
            TxtBoxCreationFicheClasseSoc.Text = "Social : Excellent";
            TxtBoxCreationFicheClasseSoc.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseMvt.Text = "Distance de mouvement : Excellente";
            TxtBoxCreationFicheClasseMvt.BackColor = Color.Orange;
            TxtBoxCreationFicheClassePre.Text = "Précision : Très bonne";
            TxtBoxCreationFicheClassePre.BackColor = Color.Green;
            TxtBoxCreationFicheClasseEsq.Text = "Esquive : Inégalée";
            TxtBoxCreationFicheClasseEsq.BackColor = Color.Red;
            TxtBoxCreationFicheClasseCrit.Text = "Taux de critique : Moyen";
            TxtBoxCreationFicheClasseCrit.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseVit.Text = "Vitesse : Bonne";
            TxtBoxCreationFicheClasseVit.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseCons.Text = "Constitution : Moyenne";
            TxtBoxCreationFicheClasseCons.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseInt.Text = "Intelligence : Très bonne";
            TxtBoxCreationFicheClasseInt.BackColor = Color.Green;
            TxtBoxCreationFicheClasseCha.Text = "Chance : Bonne";
            TxtBoxCreationFicheClasseCha.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseSau.Text = "Sauvagerie : Mauvaise";
            TxtBoxCreationFicheClasseSau.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseIns.Text = "Instinct : Excellent";
            TxtBoxCreationFicheClasseIns.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseDescription1.Text = "Une unité très fragile mais mobile qui doit compter sur son esquive pour survivre.";
            TxtBoxCreationFicheClasseDescription2.Text = "Au-delà de ça, il est le seul à pouvoir soigner ses alliés et peut OS l'ennemi avec de la chance.";
            ClassePerso.NomClasse = "Infirmier";
        }

        private void BtnCreationFicheClassePenseur_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheClassePV.Text = "PV : Moyens";
            TxtBoxCreationFicheClassePV.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseDegats.Text = "Dégâts : 1D6";
            TxtBoxCreationFicheClasseDegats.BackColor = Color.Green;
            TxtBoxCreationFicheClasseSoc.Text = "Social : Excellent";
            TxtBoxCreationFicheClasseSoc.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseMvt.Text = "Distance de mouvement : Bonne";
            TxtBoxCreationFicheClasseMvt.BackColor = Color.Yellow;
            TxtBoxCreationFicheClassePre.Text = "Précision : Mauvaise";
            TxtBoxCreationFicheClassePre.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseEsq.Text = "Esquive : Excellente";
            TxtBoxCreationFicheClasseEsq.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseCrit.Text = "Taux de critique : Abyssal";
            TxtBoxCreationFicheClasseCrit.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseVit.Text = "Vitesse : Moyenne";
            TxtBoxCreationFicheClasseVit.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseCons.Text = "Constitution : Très bonne";
            TxtBoxCreationFicheClasseCons.BackColor = Color.Green;
            TxtBoxCreationFicheClasseInt.Text = "Intelligence : Bonne";
            TxtBoxCreationFicheClasseInt.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseCha.Text = "Chance : Inégalée";
            TxtBoxCreationFicheClasseCha.BackColor = Color.Red;
            TxtBoxCreationFicheClasseSau.Text = "Sauvagerie : Très bonne";
            TxtBoxCreationFicheClasseSau.BackColor = Color.Green;
            TxtBoxCreationFicheClasseIns.Text = "Instinct : Bon";
            TxtBoxCreationFicheClasseIns.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseDescription1.Text = "Classe avec les stats les plus équilibrées, si ce n'est en précision.";
            TxtBoxCreationFicheClasseDescription2.Text = "En revanche, il est le seul à pouvoir accéder aux compétences de buff ou de debuff des armes selon son évolution.";
            ClassePerso.NomClasse = "Penseur";
        }

        private void BtnCreationFicheClasseMaso_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheClassePV.Text = "PV : Inégalés";
            TxtBoxCreationFicheClassePV.BackColor = Color.Red;
            TxtBoxCreationFicheClasseDegats.Text = "Dégâts : 1D6";
            TxtBoxCreationFicheClasseDegats.BackColor = Color.Green;
            TxtBoxCreationFicheClasseSoc.Text = "Social : Mauvais";
            TxtBoxCreationFicheClasseSoc.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseMvt.Text = "Distance de mouvement : Abyssale";
            TxtBoxCreationFicheClasseMvt.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClassePre.Text = "Précision : Moyenne";
            TxtBoxCreationFicheClassePre.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseEsq.Text = "Esquive : Abyssale";
            TxtBoxCreationFicheClasseEsq.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseCrit.Text = "Taux de critique : Moyen";
            TxtBoxCreationFicheClasseCrit.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseVit.Text = "Vitesse : Mauvaise";
            TxtBoxCreationFicheClasseVit.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseCons.Text = "Constitution : Excellente";
            TxtBoxCreationFicheClasseCons.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseInt.Text = "Intelligence : Bonne";
            TxtBoxCreationFicheClasseInt.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseCha.Text = "Chance : Très bonne";
            TxtBoxCreationFicheClasseCha.BackColor = Color.Green;
            TxtBoxCreationFicheClasseSau.Text = "Sauvagerie : Bonne";
            TxtBoxCreationFicheClasseSau.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseIns.Text = "Instinct : Très bon";
            TxtBoxCreationFicheClasseIns.BackColor = Color.Green;
            TxtBoxCreationFicheClasseDescription1.Text = "Meilleure classe défensive, capable d'attirer les attaques mais offensivement très faible.";
            TxtBoxCreationFicheClasseDescription2.Text = "Il corrige ce défaut en évoluant en pouvant utiliser ses stats défensives plutôt qu'offensives pour attaquer.";
            ClassePerso.NomClasse = "Maso";
        }

        private void BtnCreationFicheClassePrestidigitateur_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheClassePV.Text = "PV : Moyens";
            TxtBoxCreationFicheClassePV.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseDegats.Text = "Dégâts : 1D6";
            TxtBoxCreationFicheClasseDegats.BackColor = Color.Green;
            TxtBoxCreationFicheClasseSoc.Text = "Social : Excellent";
            TxtBoxCreationFicheClasseSoc.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseMvt.Text = "Distance de mouvement : Bonne";
            TxtBoxCreationFicheClasseMvt.BackColor = Color.Yellow;
            TxtBoxCreationFicheClassePre.Text = "Précision : Inégalée";
            TxtBoxCreationFicheClassePre.BackColor = Color.Red;
            TxtBoxCreationFicheClasseEsq.Text = "Esquive : Très bonne";
            TxtBoxCreationFicheClasseEsq.BackColor = Color.Green;
            TxtBoxCreationFicheClasseCrit.Text = "Taux de critique : Bon";
            TxtBoxCreationFicheClasseCrit.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseVit.Text = "Vitesse : Très bonne";
            TxtBoxCreationFicheClasseVit.BackColor = Color.Green;
            TxtBoxCreationFicheClasseCons.Text = "Constitution : Mauvaise";
            TxtBoxCreationFicheClasseCons.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseInt.Text = "Intelligence : Excellente";
            TxtBoxCreationFicheClasseInt.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseCha.Text = "Chance : Bonne";
            TxtBoxCreationFicheClasseCha.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseSau.Text = "Sauvagerie : Moyenne";
            TxtBoxCreationFicheClasseSau.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseIns.Text = "Instinct : Abyssal";
            TxtBoxCreationFicheClasseIns.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseDescription1.Text = "Guerrier populaire capable de toucher presque à coup sûr au CàC ou à distance.";
            TxtBoxCreationFicheClasseDescription2.Text = "Il peut aussi attaquer sur une zone en évoluant.";
            ClassePerso.NomClasse = "Prestidigitateur";
        }

        private void BtnCreationFicheClasseDraconiste_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheClassePV.Text = "PV : Très bons";
            TxtBoxCreationFicheClassePV.BackColor = Color.Green;
            TxtBoxCreationFicheClasseDegats.Text = "Dégâts : 1D8";
            TxtBoxCreationFicheClasseDegats.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseSoc.Text = "Social : Abyssal";
            TxtBoxCreationFicheClasseSoc.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseMvt.Text = "Distance de mouvement : Bonne";
            TxtBoxCreationFicheClasseMvt.BackColor = Color.Yellow;
            TxtBoxCreationFicheClassePre.Text = "Précision : Abyssale";
            TxtBoxCreationFicheClassePre.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseEsq.Text = "Esquive : Mauvaise";
            TxtBoxCreationFicheClasseEsq.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseCrit.Text = "Taux de critique : Excellent";
            TxtBoxCreationFicheClasseCrit.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseVit.Text = "Vitesse : Bonne";
            TxtBoxCreationFicheClasseVit.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseCons.Text = "Constitution : Bonne";
            TxtBoxCreationFicheClasseCons.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseInt.Text = "Intelligence : Moyenne";
            TxtBoxCreationFicheClasseInt.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseCha.Text = "Chance : Moyenne";
            TxtBoxCreationFicheClasseCha.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseSau.Text = "Sauvagerie : Inégalée";
            TxtBoxCreationFicheClasseSau.BackColor = Color.Red;
            TxtBoxCreationFicheClasseIns.Text = "Instinct : Très bon";
            TxtBoxCreationFicheClasseIns.BackColor = Color.Green;
            TxtBoxCreationFicheClasseDescription1.Text = "Le guerrier le plus bourrin qui possède une précision terrible.";
            TxtBoxCreationFicheClasseDescription2.Text = "Mais il peut l'améliorer en éliminant des ennemis, se soigner et diviser par deux certains dégâts reçus en évoluant.";
            ClassePerso.NomClasse = "Draconiste";
        }

        private void BtnCreationFicheClasseIllumine_Click(object sender, EventArgs e)
        {
            TxtBoxCreationFicheClassePV.Text = "PV : Excellents";
            TxtBoxCreationFicheClassePV.BackColor = Color.Orange;
            TxtBoxCreationFicheClasseDegats.Text = "Dégâts : 1D6";
            TxtBoxCreationFicheClasseDegats.BackColor = Color.Green;
            TxtBoxCreationFicheClasseSoc.Text = "Social : Inégalé";
            TxtBoxCreationFicheClasseSoc.BackColor = Color.Red;
            TxtBoxCreationFicheClasseMvt.Text = "Distance de mouvement : Inégalée";
            TxtBoxCreationFicheClasseMvt.BackColor = Color.Red;
            TxtBoxCreationFicheClassePre.Text = "Précision : Bonne";
            TxtBoxCreationFicheClassePre.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseEsq.Text = "Esquive : Bonne";
            TxtBoxCreationFicheClasseEsq.BackColor = Color.Yellow;
            TxtBoxCreationFicheClasseCrit.Text = "Taux de critique : Inégalé";
            TxtBoxCreationFicheClasseCrit.BackColor = Color.Red;
            TxtBoxCreationFicheClasseVit.Text = "Vitesse : Très bonne";
            TxtBoxCreationFicheClasseVit.BackColor = Color.Green;
            TxtBoxCreationFicheClasseCons.Text = "Constitution : Moyenne";
            TxtBoxCreationFicheClasseCons.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseInt.Text = "Intelligence : Mauvaise";
            TxtBoxCreationFicheClasseInt.BackColor = Color.CornflowerBlue;
            TxtBoxCreationFicheClasseCha.Text = "Chance : Très bonne";
            TxtBoxCreationFicheClasseCha.BackColor = Color.Green;
            TxtBoxCreationFicheClasseSau.Text = "Sauvagerie : Abyssale";
            TxtBoxCreationFicheClasseSau.BackColor = Color.MediumPurple;
            TxtBoxCreationFicheClasseIns.Text = "Instinct : Moyen";
            TxtBoxCreationFicheClasseIns.BackColor = Color.LightBlue;
            TxtBoxCreationFicheClasseDescription1.Text = "Icone possédant la capacité de taper très fort grâce à sa stat de Crit.";
            TxtBoxCreationFicheClasseDescription2.Text = "Il a aussi le meilleur potentiel de Social et peut permettre à ses alliés de rejouer en évoluant.";
            ClassePerso.NomClasse = "Illumine";
        }

        private void BtnCreationFicheClasseRetour_Click(object sender, EventArgs e)
        {
            PnlCreationFicheClasse.Visible = false;
            PnlCreationFicheAge.Visible = true;
        }

        private void BtnCreationFicheClasseSuivant_Click(object sender, EventArgs e)
        {
            if((ClassePerso.NomClasse == "Vagabond") || (ClassePerso.NomClasse == "Combattant") || (ClassePerso.NomClasse == "Arcaniste") || (ClassePerso.NomClasse == "Maso") || (ClassePerso.NomClasse == "Prestidigitateur") || (ClassePerso.NomClasse == "Serviteur") || (ClassePerso.NomClasse == "Infirmier") || (ClassePerso.NomClasse == "Draconiste") || (ClassePerso.NomClasse == "Penseur") || (ClassePerso.NomClasse == "Illumine"))
            {
                initClasse(ClassePerso.NomClasse, ref ClassePerso, ref InventaireJoueur);

                PnlCreationFicheClasse.Visible = false;
                PnlCreationFicheSocial.Location = new Point(0, 0);
                PnlCreationFicheSocial.Visible = true;
            }
        }

        //Panel Creation Fiche Social

        private void BtnCreationFicheSocialRetour_Click(object sender, EventArgs e)
        {
            PnlCreationFicheSocial.Visible = false;
            PnlCreationFicheClasse.Visible = true;
        }

        private void BtnCreationFicheSocialSuivant_Click(object sender, EventArgs e)
        {
            if(TxtBoxCreationFicheSocial.Text != "")
            {
                ClassePerso.SocialBase = Int32.Parse(TxtBoxCreationFicheSocial.Text) + BonusSoc;
                ClassePerso.SocialTot = ClassePerso.SocialBase;

                PnlCreationFicheSocial.Visible = false;
                PnlCreationFicheMarché.Location = new Point(0, 0);
                PnlCreationFicheMarché.Visible = true;
            }
        }

        //Panel Creation Fiche Marché

        private void BtnCreationFicheMarché_Click(object sender, EventArgs e)
        {
            PnlCreationFicheMarché.Visible = false;
            PnlMarchéAchatVente.Location = new Point(0, 0);
            PnlMarchéAchatVente.Visible = true;

            actualisationLblFondsMarché();
        }
        private void BtnCreationFicheMarchéRetour_Click(object sender, EventArgs e)
        {
            PnlCreationFicheMarché.Visible = false;
            PnlCreationFicheSocial.Visible = true;
        }

        private void BtnCreationFicheMarchéTerminer_Click(object sender, EventArgs e)
        {
            SauvegardeFichePerso();
            PnlCreationFicheMarché.Visible = false;
            flag_marche_creation_perso = false;
        }

        //Bouton Load Fiche Existante

        private void BtnLoadFiche_Click(object sender, EventArgs e)
        {
            PnlFiche.Location = new Point(0, 0);
            PnlFiche.Visible = true;
            initInventaire(ref Inventaire);
            initComp(ref Competences);
            ChargerFichePerso();
            calculStatsTot();
            apparitionBtnPhase();
            actualisationLblFichePerso();
        }

        //Panel Fiche Perso

        private void BtnSaveData_Click(object sender, EventArgs e)
        {
            SauvegardeFichePerso();
        }

        private void BtnLoadData_Click(object sender, EventArgs e)
        {
            ChargerFichePerso();
            actualisationLblFichePerso();
        }

        private void BtnCalendrier_Click(object sender, EventArgs e)
        {

        }

        private void BtnChgtPhase_Click(object sender, EventArgs e)
        {
            if(Phase == "d'enquete")
            {
                Phase = "de combat";
                tour_combat = 1;
                apparitionBtnPhase();
                gainExpCombat();
                actualisationLblFichePerso();
            }
            else
            {
                Phase = "d'enquete";
                tour_combat = 0;
                ClassePerso.PVTotAct = ClassePerso.PVTotMax;
                apparitionBtnPhase();
                chgtDate();
                actualisationLblFichePerso();
            }
        }

        private void BtnAttribuerPtsComp_Click(object sender, EventArgs e)
        {
            PnlAttribPtsComp.Location = new Point(0, 0);
            PnlAttribPtsComp.Visible = true;
            PnlFiche.Visible = false;

            PtsCompApres = PtsComp;
            ptPV = 0;
            ptPre = 0;
            ptEsq = 0;
            ptCrit = 0;
            ptVit = 0;
            ptCons = 0;
            ptInt = 0;
            ptCha = 0;
            ptSau = 0;
            ptIns = 0;
            TxtBoxAttribPtsComp.Text = "";
            actualisationLblAttribPtsComp();
        }

        private void BtnInfosEnnemis_Click(object sender, EventArgs e)
        {

        }

        private void BtnUtilisationObjComp_Click(object sender, EventArgs e)
        {
            PnlObjComp.Location = new Point(0, 0);
            PnlObjComp.Visible = true;
            PnlFiche.Visible = false;
        }

        private void BtnEffetRecu_Click(object sender, EventArgs e)
        {
            PnlEffetRecu.Location = new Point(0, 0);
            PnlEffetRecu.Visible = true;
            PnlFiche.Visible = false;
        }

        private void BtnFinTour_Click(object sender, EventArgs e)
        {
            tour_combat++;
        }

        private void BtnPlanificationActivité_Click(object sender, EventArgs e)
        {
            PnlFiche.Visible = false;
            PnlChxActivité.Location = new Point(0, 0);
            PnlChxActivité.Visible = true;
        }

        private void BtnFinPériode_Click(object sender, EventArgs e)
        {
            if(chx_activite != "Rien")
            {
                switch(chx_activite)
                {
                    case "Travail":
                        calculMultiplicateurArgent();
                        Fonds += (int)(Chapitre * 200 * MultiplicateurArgent);
                        break;
                    case "Entrainement":
                        gainExpEntrainement();
                        break;
                    case "Marche":
                        PnlMarchéAchatVente.Location = new Point(0, 0);
                        PnlMarchéAchatVente.Visible = true;
                        PnlFiche.Visible = false;
                        break;
                    default:
                        break;
                }

                chgtDate();
                actualisationLblFichePerso();
                TxtBoxChxActivitéInfos.Text = "";
                TxtBoxChxActivitéChoixEffectué.Text = "";
                chx_activite = "Rien";
            }
        }

        //Panel Choix Activité

        private void BtnChxActivitéTravail_Click(object sender, EventArgs e)
        {
            chx_activite = "Travail";
            TxtBoxChxActivitéInfos.Text = "Gagner de l'argent en effectuant un travail en rapport avec l'âge, le statut et la classe. Possibilité de récupérer des infos selon la situation.";
            actualisationTxtBoxChxActivitéChxEffectué();
        }

        private void BtnChxActivitéEntraînement_Click(object sender, EventArgs e)
        {
            chx_activite = "Entrainement";
            TxtBoxChxActivitéInfos.Text = "Gagner des points d'exp en s'entraînant.";
            actualisationTxtBoxChxActivitéChxEffectué();
        }

        private void BtnChxActivitéMarché_Click(object sender, EventArgs e)
        {
            chx_activite = "Marche";
            TxtBoxChxActivitéInfos.Text = "Accéder au marché afin d'acheter ou de vendre des objets.";
            actualisationTxtBoxChxActivitéChxEffectué();
            actualisationLblFondsMarché();
        }

        private void BtnChxActivitéRechercheInfos_Click(object sender, EventArgs e)
        {
            chx_activite = "Recherche Infos";
            TxtBoxChxActivitéInfos.Text = "Rechercher et discuter afin d'obtenir des infos sur la mission en cours.";
            actualisationTxtBoxChxActivitéChxEffectué();
        }

        private void BtnChxActivitéEventSpécial_Click(object sender, EventArgs e)
        {
            chx_activite = "Evenement Special";
            TxtBoxChxActivitéInfos.Text = "Accéder à un évènement spécial en lien avec la date.";
            actualisationTxtBoxChxActivitéChxEffectué();
        }

        private void BtnChxActivitéRetour_Click(object sender, EventArgs e)
        {
            PnlChxActivité.Visible = false;
            PnlFiche.Visible = true;
            actualisationLblFichePerso();
        }

        //Panel Marché Achat Vente

        private void BtnMarchéAchatVenteAcheter_Click(object sender, EventArgs e)
        {
            selecRareteChapitre();

            PnlMarchéAchatVente.Visible = false;
            PnlMarchéAchatChoixType.Location = new Point(0, 0);
            PnlMarchéAchatChoixType.Visible = true;
        }

        private void BtnMarchéAchatVenteVendre_Click(object sender, EventArgs e)
        {
            if (InventaireJoueur[0].EmplOccupe == false && InventaireJoueur[1].EmplOccupe == false && InventaireJoueur[2].EmplOccupe == false && InventaireJoueur[3].EmplOccupe == false && InventaireJoueur[4].EmplOccupe == false && InventaireJoueur[5].EmplOccupe == false && InventaireJoueur[6].EmplOccupe == false && InventaireJoueur[7].EmplOccupe == false)
            {
                LblMarchéAchatVenteInvVide.Visible = true;
            }
            else
            {
                PnlMarchéAchatVente.Visible = false;
                PnlMarchéVente.Location = new Point(0, 0);
                PnlMarchéVente.Visible = true;

                actualisationObjetsVente(ref InventaireJoueur);
            }
        }

        private void BtnMarchéAchatVenteQuitter_Click(object sender, EventArgs e)
        {
            PnlMarchéAchatVente.Visible = false;

            if (flag_marche_creation_perso)
            {
                PnlCreationFicheMarché.Visible = true;
            }
            else
            {
                PnlFiche.Visible = true;
            }

            calculStatsTot();
            actualisationLblFichePerso();
        }

        //Panel Marché Achat Choix Type

        private void BtnMarchéAchatChxTypeRetour_Click(object sender, EventArgs e)
        {
            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatVente.Visible = true;
        }

        private void BtnMarchéAchatChxTypeEpées_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Epee";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeLances_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Lance";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeDagues_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Dague";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeOutils_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Outil";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeTranscendance_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Transcendance";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeArcs_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Arc";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeTomes_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Tome";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeLancers_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Lancers";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeArmesAFeu_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Arme a feu";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeFrondes_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Fronde";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeHaches_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Hache";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeBoucliers_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Bouclier";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypePoutres_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Poutre";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypePugilats_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Pugilats";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeBestipierres_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Bestipierre";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeObjSoins_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Soin";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeObjDégâts_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Degats";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeObjBuffs_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Buff";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeObjDebuffs_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Debuff";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeComp_Click(object sender, EventArgs e)
        {
            marché_achat_type = "Comp";

            PnlMarchéAchatChoixType.Visible = false;
            PnlMarchéAchatChoixRareté.Location = new Point(0, 0);
            PnlMarchéAchatChoixRareté.Visible = true;
        }

        private void BtnMarchéAchatChxTypeMaitriseArmes_Click(object sender, EventArgs e)
        {

        }

        //Panel Marché Achat Choix Rareté

        private void BtnMarchéAchatChxRaretéRetour_Click(object sender, EventArgs e)
        {
            PnlMarchéAchatChoixRareté.Visible = false;
            PnlMarchéAchatChoixType.Visible = true;
        }

        private void BtnMarchéAchatChxRareté1_Click(object sender, EventArgs e)
        {
            marché_achat_rarete = "1";
            actualisationObjetsAchat(ref ObjetsMarché, ref Inventaire, marché_achat_type, marché_achat_rarete);

            PnlMarchéAchatChoixRareté.Visible = false;
            PnlMarchéAchat.Location = new Point(0, 0);
            PnlMarchéAchat.Visible = true;
        }

        private void BtnMarchéAchatChxRareté2_Click(object sender, EventArgs e)
        {
            marché_achat_rarete = "2";
            actualisationObjetsAchat(ref ObjetsMarché, ref Inventaire, marché_achat_type, marché_achat_rarete);

            PnlMarchéAchatChoixRareté.Visible = false;
            PnlMarchéAchat.Location = new Point(0, 0);
            PnlMarchéAchat.Visible = true;
        }

        private void BtnMarchéAchatChxRareté3_Click(object sender, EventArgs e)
        {
            marché_achat_rarete = "3";
            actualisationObjetsAchat(ref ObjetsMarché, ref Inventaire, marché_achat_type, marché_achat_rarete);

            PnlMarchéAchatChoixRareté.Visible = false;
            PnlMarchéAchat.Location = new Point(0, 0);
            PnlMarchéAchat.Visible = true;
        }

        private void BtnMarchéAchatChxRareté4_Click(object sender, EventArgs e)
        {
            marché_achat_rarete = "4";
            actualisationObjetsAchat(ref ObjetsMarché, ref Inventaire, marché_achat_type, marché_achat_rarete);

            PnlMarchéAchatChoixRareté.Visible = false;
            PnlMarchéAchat.Location = new Point(0, 0);
            PnlMarchéAchat.Visible = true;
        }

        private void BtnMarchéAchatChxRareté5_Click(object sender, EventArgs e)
        {
            marché_achat_rarete = "5";
            actualisationObjetsAchat(ref ObjetsMarché, ref Inventaire, marché_achat_type, marché_achat_rarete);

            PnlMarchéAchatChoixRareté.Visible = false;
            PnlMarchéAchat.Location = new Point(0, 0);
            PnlMarchéAchat.Visible = true;
        }

        private void BtnMarchéAchatChxRaretéEX_Click(object sender, EventArgs e)
        {
            marché_achat_rarete = "EX";
            actualisationObjetsAchat(ref ObjetsMarché, ref Inventaire, marché_achat_type, marché_achat_rarete);

            PnlMarchéAchatChoixRareté.Visible = false;
            PnlMarchéAchat.Location = new Point(0, 0);
            PnlMarchéAchat.Visible = true;
        }

        //Panel Marché Achat

        private void BtnMarchéAchatRetour_Click(object sender, EventArgs e)
        {
            PnlMarchéAchat.Visible = false;
            PnlMarchéAchatChoixRareté.Visible = true;
            LblMarchéAchatErreur.Visible = false;
        }

        private void BtnMarchéAchatObj1_Click(object sender, EventArgs e)
        {
            flag_achat_conso_double = false;

            if ((InventaireJoueur[0].Nom == ObjetsMarché[0].Nom || InventaireJoueur[1].Nom == ObjetsMarché[0].Nom || InventaireJoueur[2].Nom == ObjetsMarché[0].Nom || InventaireJoueur[3].Nom == ObjetsMarché[0].Nom || InventaireJoueur[4].Nom == ObjetsMarché[0].Nom || InventaireJoueur[5].Nom == ObjetsMarché[0].Nom || InventaireJoueur[6].Nom == ObjetsMarché[0].Nom || InventaireJoueur[7].Nom == ObjetsMarché[0].Nom) && (ObjetsMarché[0].Type == "Soin" || ObjetsMarché[0].Type == "Degats" || ObjetsMarché[0].Type == "Buff" || ObjetsMarché[0].Type == "Debuff"))
            {
                int i = 0;
                nb_consos_meme_nom = 0;

                while (i < 8)
                {
                    if (InventaireJoueur[i].Nom == ObjetsMarché[0].Nom)
                    {
                        ConsommablesDoubles[nb_consos_meme_nom] = i;
                        nb_consos_meme_nom++;
                    }

                    i++;
                }

                i = 0;

                while (i < nb_consos_meme_nom && flag_achat_conso_double == false)
                {
                    if (InventaireJoueur[ConsommablesDoubles[i]].Quantité < 3)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur[ConsommablesDoubles[i]], ref ObjetsMarché[0]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[0].Nom + " acheté(e).";
                        flag_achat_conso_double = true;
                    }

                    i++;
                }

            }

            if (flag_achat_conso_double == false)
            {
                if (InventaireJoueur[0].EmplOccupe == false || InventaireJoueur[1].EmplOccupe == false || InventaireJoueur[2].EmplOccupe == false || InventaireJoueur[3].EmplOccupe == false || InventaireJoueur[4].EmplOccupe == false || InventaireJoueur[5].EmplOccupe == false || InventaireJoueur[6].EmplOccupe == false || InventaireJoueur[7].EmplOccupe == false)
                {
                    if (Fonds >= ObjetsMarché[0].Prix)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur, ref ObjetsMarché[0]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[0].Nom + " acheté(e).";
                    }
                    else
                    {
                        LblMarchéAchatErreur.Text = "Fonds insuffisants.";
                        LblMarchéAchatErreur.Visible = true;
                    }
                }
                else
                {
                    LblMarchéAchatErreur.Text = "Vous n'avez plus de place dans votre inventaire.";
                    LblMarchéAchatErreur.Visible = true;
                }
            }
        }

        private void BtnMarchéAchatObj2_Click(object sender, EventArgs e)
        {
            flag_achat_conso_double = false;

            if ((InventaireJoueur[0].Nom == ObjetsMarché[1].Nom || InventaireJoueur[1].Nom == ObjetsMarché[1].Nom || InventaireJoueur[2].Nom == ObjetsMarché[1].Nom || InventaireJoueur[3].Nom == ObjetsMarché[1].Nom || InventaireJoueur[4].Nom == ObjetsMarché[1].Nom || InventaireJoueur[5].Nom == ObjetsMarché[1].Nom || InventaireJoueur[6].Nom == ObjetsMarché[1].Nom || InventaireJoueur[7].Nom == ObjetsMarché[1].Nom) && (ObjetsMarché[1].Type == "Soin" || ObjetsMarché[1].Type == "Degats" || ObjetsMarché[1].Type == "Buff" || ObjetsMarché[1].Type == "Debuff"))
            {
                int i = 0;
                nb_consos_meme_nom = 0;

                while (i < 8)
                {
                    if (InventaireJoueur[i].Nom == ObjetsMarché[1].Nom)
                    {
                        ConsommablesDoubles[nb_consos_meme_nom] = i;
                        nb_consos_meme_nom++;
                    }

                    i++;
                }

                i = 0;

                while (i < nb_consos_meme_nom && flag_achat_conso_double == false)
                {
                    if (InventaireJoueur[ConsommablesDoubles[i]].Quantité < 3)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur[ConsommablesDoubles[i]], ref ObjetsMarché[1]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[1].Nom + " acheté(e).";
                        flag_achat_conso_double = true;
                    }

                    i++;
                }

            }

            if (flag_achat_conso_double == false)
            {
                if (InventaireJoueur[0].EmplOccupe == false || InventaireJoueur[1].EmplOccupe == false || InventaireJoueur[2].EmplOccupe == false || InventaireJoueur[3].EmplOccupe == false || InventaireJoueur[4].EmplOccupe == false || InventaireJoueur[5].EmplOccupe == false || InventaireJoueur[6].EmplOccupe == false || InventaireJoueur[7].EmplOccupe == false)
                {
                    if (Fonds >= ObjetsMarché[1].Prix)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur, ref ObjetsMarché[1]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[1].Nom + " acheté(e).";
                    }
                    else
                    {
                        LblMarchéAchatErreur.Text = "Fonds insuffisants.";
                        LblMarchéAchatErreur.Visible = true;
                    }
                }
                else
                {
                    LblMarchéAchatErreur.Text = "Vous n'avez plus de place dans votre inventaire.";
                    LblMarchéAchatErreur.Visible = true;
                }
            }
        }

        private void BtnMarchéAchatObj3_Click(object sender, EventArgs e)
        {
            flag_achat_conso_double = false;

            if ((InventaireJoueur[0].Nom == ObjetsMarché[2].Nom || InventaireJoueur[1].Nom == ObjetsMarché[2].Nom || InventaireJoueur[2].Nom == ObjetsMarché[2].Nom || InventaireJoueur[3].Nom == ObjetsMarché[2].Nom || InventaireJoueur[4].Nom == ObjetsMarché[2].Nom || InventaireJoueur[5].Nom == ObjetsMarché[2].Nom || InventaireJoueur[6].Nom == ObjetsMarché[2].Nom || InventaireJoueur[7].Nom == ObjetsMarché[2].Nom) && (ObjetsMarché[2].Type == "Soin" || ObjetsMarché[2].Type == "Degats" || ObjetsMarché[2].Type == "Buff" || ObjetsMarché[2].Type == "Debuff"))
            {
                int i = 0;
                nb_consos_meme_nom = 0;

                while (i < 8)
                {
                    if (InventaireJoueur[i].Nom == ObjetsMarché[2].Nom)
                    {
                        ConsommablesDoubles[nb_consos_meme_nom] = i;
                        nb_consos_meme_nom++;
                    }

                    i++;
                }

                i = 0;

                while (i < nb_consos_meme_nom && flag_achat_conso_double == false)
                {
                    if (InventaireJoueur[ConsommablesDoubles[i]].Quantité < 3)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur[ConsommablesDoubles[i]], ref ObjetsMarché[2]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[2].Nom + " acheté(e).";
                        flag_achat_conso_double = true;
                    }

                    i++;
                }

            }

            if (flag_achat_conso_double == false)
            {
                if (InventaireJoueur[0].EmplOccupe == false || InventaireJoueur[1].EmplOccupe == false || InventaireJoueur[2].EmplOccupe == false || InventaireJoueur[3].EmplOccupe == false || InventaireJoueur[4].EmplOccupe == false || InventaireJoueur[5].EmplOccupe == false || InventaireJoueur[6].EmplOccupe == false || InventaireJoueur[7].EmplOccupe == false)
                {
                    if (Fonds >= ObjetsMarché[2].Prix)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur, ref ObjetsMarché[2]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[2].Nom + " acheté(e).";
                    }
                    else
                    {
                        LblMarchéAchatErreur.Text = "Fonds insuffisants.";
                        LblMarchéAchatErreur.Visible = true;
                    }
                }
                else
                {
                    LblMarchéAchatErreur.Text = "Vous n'avez plus de place dans votre inventaire.";
                    LblMarchéAchatErreur.Visible = true;
                }
            }
        }

        private void BtnMarchéAchatObj4_Click(object sender, EventArgs e)
        {
            flag_achat_conso_double = false;

            if ((InventaireJoueur[0].Nom == ObjetsMarché[3].Nom || InventaireJoueur[1].Nom == ObjetsMarché[3].Nom || InventaireJoueur[2].Nom == ObjetsMarché[3].Nom || InventaireJoueur[3].Nom == ObjetsMarché[3].Nom || InventaireJoueur[4].Nom == ObjetsMarché[3].Nom || InventaireJoueur[5].Nom == ObjetsMarché[3].Nom || InventaireJoueur[6].Nom == ObjetsMarché[3].Nom || InventaireJoueur[7].Nom == ObjetsMarché[3].Nom) && (ObjetsMarché[3].Type == "Soin" || ObjetsMarché[3].Type == "Degats" || ObjetsMarché[3].Type == "Buff" || ObjetsMarché[3].Type == "Debuff"))
            {
                int i = 0;
                nb_consos_meme_nom = 0;

                while (i < 8)
                {
                    if (InventaireJoueur[i].Nom == ObjetsMarché[3].Nom)
                    {
                        ConsommablesDoubles[nb_consos_meme_nom] = i;
                        nb_consos_meme_nom++;
                    }

                    i++;
                }

                i = 0;

                while (i < nb_consos_meme_nom && flag_achat_conso_double == false)
                {
                    if (InventaireJoueur[ConsommablesDoubles[i]].Quantité < 3)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur[ConsommablesDoubles[i]], ref ObjetsMarché[3]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[3].Nom + " acheté(e).";
                        flag_achat_conso_double = true;
                    }

                    i++;
                }

            }

            if (flag_achat_conso_double == false)
            {
                if (InventaireJoueur[0].EmplOccupe == false || InventaireJoueur[1].EmplOccupe == false || InventaireJoueur[2].EmplOccupe == false || InventaireJoueur[3].EmplOccupe == false || InventaireJoueur[4].EmplOccupe == false || InventaireJoueur[5].EmplOccupe == false || InventaireJoueur[6].EmplOccupe == false || InventaireJoueur[7].EmplOccupe == false)
                {
                    if (Fonds >= ObjetsMarché[3].Prix)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur, ref ObjetsMarché[3]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[3].Nom + " acheté(e).";
                    }
                    else
                    {
                        LblMarchéAchatErreur.Text = "Fonds insuffisants.";
                        LblMarchéAchatErreur.Visible = true;
                    }
                }
                else
                {
                    LblMarchéAchatErreur.Text = "Vous n'avez plus de place dans votre inventaire.";
                    LblMarchéAchatErreur.Visible = true;
                }
            }
        }

        private void BtnMarchéAchatObj5_Click(object sender, EventArgs e)
        {
            flag_achat_conso_double = false;

            if ((InventaireJoueur[0].Nom == ObjetsMarché[4].Nom || InventaireJoueur[1].Nom == ObjetsMarché[4].Nom || InventaireJoueur[2].Nom == ObjetsMarché[4].Nom || InventaireJoueur[3].Nom == ObjetsMarché[4].Nom || InventaireJoueur[4].Nom == ObjetsMarché[4].Nom || InventaireJoueur[5].Nom == ObjetsMarché[4].Nom || InventaireJoueur[6].Nom == ObjetsMarché[4].Nom || InventaireJoueur[7].Nom == ObjetsMarché[4].Nom) && (ObjetsMarché[4].Type == "Soin" || ObjetsMarché[4].Type == "Degats" || ObjetsMarché[4].Type == "Buff" || ObjetsMarché[4].Type == "Debuff"))
            {
                int i = 0;
                nb_consos_meme_nom = 0;

                while (i < 8)
                {
                    if (InventaireJoueur[i].Nom == ObjetsMarché[4].Nom)
                    {
                        ConsommablesDoubles[nb_consos_meme_nom] = i;
                        nb_consos_meme_nom++;
                    }

                    i++;
                }

                i = 0;

                while (i < nb_consos_meme_nom && flag_achat_conso_double == false)
                {
                    if (InventaireJoueur[ConsommablesDoubles[i]].Quantité < 3)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur[ConsommablesDoubles[i]], ref ObjetsMarché[4]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[4].Nom + " acheté(e).";
                        flag_achat_conso_double = true;
                    }

                    i++;
                }

            }

            if (flag_achat_conso_double == false)
            {
                if (InventaireJoueur[0].EmplOccupe == false || InventaireJoueur[1].EmplOccupe == false || InventaireJoueur[2].EmplOccupe == false || InventaireJoueur[3].EmplOccupe == false || InventaireJoueur[4].EmplOccupe == false || InventaireJoueur[5].EmplOccupe == false || InventaireJoueur[6].EmplOccupe == false || InventaireJoueur[7].EmplOccupe == false)
                {
                    if (Fonds >= ObjetsMarché[4].Prix)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur, ref ObjetsMarché[4]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[4].Nom + " acheté(e).";
                    }
                    else
                    {
                        LblMarchéAchatErreur.Text = "Fonds insuffisants.";
                        LblMarchéAchatErreur.Visible = true;
                    }
                }
                else
                {
                    LblMarchéAchatErreur.Text = "Vous n'avez plus de place dans votre inventaire.";
                    LblMarchéAchatErreur.Visible = true;
                }
            }
        }

        private void BtnMarchéAchatObj6_Click(object sender, EventArgs e)
        {
            flag_achat_conso_double = false;

            if ((InventaireJoueur[0].Nom == ObjetsMarché[5].Nom || InventaireJoueur[1].Nom == ObjetsMarché[5].Nom || InventaireJoueur[2].Nom == ObjetsMarché[5].Nom || InventaireJoueur[3].Nom == ObjetsMarché[5].Nom || InventaireJoueur[4].Nom == ObjetsMarché[5].Nom || InventaireJoueur[5].Nom == ObjetsMarché[5].Nom || InventaireJoueur[6].Nom == ObjetsMarché[5].Nom || InventaireJoueur[7].Nom == ObjetsMarché[5].Nom) && (ObjetsMarché[5].Type == "Soin" || ObjetsMarché[5].Type == "Degats" || ObjetsMarché[5].Type == "Buff" || ObjetsMarché[5].Type == "Debuff"))
            {
                int i = 0;
                nb_consos_meme_nom = 0;
                
                while(i < 8)
                {
                    if (InventaireJoueur[i].Nom == ObjetsMarché[5].Nom)
                    {
                        ConsommablesDoubles[nb_consos_meme_nom] = i;
                        nb_consos_meme_nom++;
                    }

                    i++;
                }

                i = 0;

                while (i < nb_consos_meme_nom && flag_achat_conso_double == false)
                {
                    if (InventaireJoueur[ConsommablesDoubles[i]].Quantité < 3)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur[ConsommablesDoubles[i]], ref ObjetsMarché[5]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[5].Nom + " acheté(e).";
                        flag_achat_conso_double = true;
                    }

                    i++;
                }

            }

            if (flag_achat_conso_double == false)
            {
                if (InventaireJoueur[0].EmplOccupe == false || InventaireJoueur[1].EmplOccupe == false || InventaireJoueur[2].EmplOccupe == false || InventaireJoueur[3].EmplOccupe == false || InventaireJoueur[4].EmplOccupe == false || InventaireJoueur[5].EmplOccupe == false || InventaireJoueur[6].EmplOccupe == false || InventaireJoueur[7].EmplOccupe == false)
                {
                    if (Fonds >= ObjetsMarché[5].Prix)
                    {
                        achatObjetMarché(ref Fonds, ref InventaireJoueur, ref ObjetsMarché[5]);
                        LblMarchéAchatErreur.Visible = true;
                        LblMarchéAchatErreur.Text = ObjetsMarché[5].Nom + " acheté(e).";
                    }
                    else
                    {
                        LblMarchéAchatErreur.Text = "Fonds insuffisants.";
                        LblMarchéAchatErreur.Visible = true;
                    }
                }
                else
                {
                    LblMarchéAchatErreur.Text = "Vous n'avez plus de place dans votre inventaire.";
                    LblMarchéAchatErreur.Visible = true;
                }
            }
        }

        //Panel Marché Vente

        private void BtnMarchéVenteObj1_Click(object sender, EventArgs e)
        {
            venteObjetMarché(ref Fonds, ref InventaireJoueur[0]);

            if (InventaireJoueur[0].Quantité == 0)
            {
                BtnMarchéVenteObj1.Visible = false;
                LblMarchéVenteBonusObj1.Visible = false;
                LblMarchéVentePortéeObj1.Visible = false;
                LblMarchéVentePrixObj1.Visible = false;
                LblMarchéVenteEffetsSupObj1.Visible = false;
            }
            else
            {
                actualisationObjetsVente(ref InventaireJoueur);
            }
        }

        private void BtnMarchéVenteObj2_Click(object sender, EventArgs e)
        {
            venteObjetMarché(ref Fonds, ref InventaireJoueur[1]);

            if (InventaireJoueur[1].Quantité == 0)
            {
                BtnMarchéVenteObj2.Visible = false;
                LblMarchéVenteBonusObj2.Visible = false;
                LblMarchéVentePortéeObj2.Visible = false;
                LblMarchéVentePrixObj2.Visible = false;
                LblMarchéVenteEffetsSupObj2.Visible = false;
            }
            else
            {
                actualisationObjetsVente(ref InventaireJoueur);
            }
        }

        private void BtnMarchéVenteObj3_Click(object sender, EventArgs e)
        {
            venteObjetMarché(ref Fonds, ref InventaireJoueur[2]);

            if (InventaireJoueur[2].Quantité == 0)
            {
                BtnMarchéVenteObj3.Visible = false;
                LblMarchéVenteBonusObj3.Visible = false;
                LblMarchéVentePortéeObj3.Visible = false;
                LblMarchéVentePrixObj3.Visible = false;
                LblMarchéVenteEffetsSupObj3.Visible = false;
            }
            else
            {
                actualisationObjetsVente(ref InventaireJoueur);
            }
        }

        private void BtnMarchéVenteObj4_Click(object sender, EventArgs e)
        {
            venteObjetMarché(ref Fonds, ref InventaireJoueur[3]);

            if (InventaireJoueur[3].Quantité == 0)
            {
                BtnMarchéVenteObj4.Visible = false;
                LblMarchéVenteBonusObj4.Visible = false;
                LblMarchéVentePortéeObj4.Visible = false;
                LblMarchéVentePrixObj4.Visible = false;
                LblMarchéVenteEffetsSupObj4.Visible = false;
            }
            else
            {
                actualisationObjetsVente(ref InventaireJoueur);
            }
        }

        private void BtnMarchéVenteObj5_Click(object sender, EventArgs e)
        {
            venteObjetMarché(ref Fonds, ref InventaireJoueur[4]);

            if (InventaireJoueur[4].Quantité == 0)
            {
                BtnMarchéVenteObj5.Visible = false;
                LblMarchéVenteBonusObj5.Visible = false;
                LblMarchéVentePortéeObj5.Visible = false;
                LblMarchéVentePrixObj5.Visible = false;
                LblMarchéVenteEffetsSupObj5.Visible = false;
            }
            else
            {
                actualisationObjetsVente(ref InventaireJoueur);
            }
        }

        private void BtnMarchéVenteObj6_Click(object sender, EventArgs e)
        {
            venteObjetMarché(ref Fonds, ref InventaireJoueur[5]);

            if (InventaireJoueur[5].Quantité == 0)
            {
                BtnMarchéVenteObj6.Visible = false;
                LblMarchéVenteBonusObj6.Visible = false;
                LblMarchéVentePortéeObj6.Visible = false;
                LblMarchéVentePrixObj6.Visible = false;
                LblMarchéVenteEffetsSupObj6.Visible = false;
            }
            else
            {
                actualisationObjetsVente(ref InventaireJoueur);
            }
        }

        private void BtnMarchéVenteObj7_Click(object sender, EventArgs e)
        {
            venteObjetMarché(ref Fonds, ref InventaireJoueur[6]);

            if (InventaireJoueur[6].Quantité == 0)
            {
                BtnMarchéVenteObj7.Visible = false;
                LblMarchéVenteBonusObj7.Visible = false;
                LblMarchéVentePortéeObj7.Visible = false;
                LblMarchéVentePrixObj7.Visible = false;
                LblMarchéVenteEffetsSupObj7.Visible = false;
            }
            else
            {
                actualisationObjetsVente(ref InventaireJoueur);
            }
        }

        private void BtnMarchéVenteObj8_Click(object sender, EventArgs e)
        {
            venteObjetMarché(ref Fonds, ref InventaireJoueur[7]);

            if (InventaireJoueur[7].Quantité == 0)
            {
                BtnMarchéVenteObj8.Visible = false;
                LblMarchéVenteBonusObj8.Visible = false;
                LblMarchéVentePortéeObj8.Visible = false;
                LblMarchéVentePrixObj8.Visible = false;
                LblMarchéVenteEffetsSupObj8.Visible = false;
            }
            else
            {
                actualisationObjetsVente(ref InventaireJoueur);
            }
        }

        private void BtnMarchéVenteRetour_Click(object sender, EventArgs e)
        {
            PnlMarchéVente.Visible = false;
            PnlMarchéAchatVente.Visible = true;
        }

        //Panel Attribution Pts Comp

        private void BtnAttribPtsCompRetour_Click(object sender, EventArgs e)
        {
            PnlAttribPtsComp.Visible = false;
            PnlFiche.Visible = true;
            actualisationLblFichePerso();
        }

        private void BtnAttribPtsCompValider_Click(object sender, EventArgs e)
        {
            ClassePerso.PVBase += ptPV;
            ClassePerso.PreBase += ptPre;
            ClassePerso.EsqBase += ptEsq;
            ClassePerso.CritBase += ptCrit;
            ClassePerso.VitBase += ptVit;
            ClassePerso.ConsBase += ptCons;
            ClassePerso.IntBase += ptInt;
            ClassePerso.ChaBase += ptCha;
            ClassePerso.SauBase += ptSau;
            ClassePerso.InsBase += ptIns;
            PtsComp = PtsCompApres;

            PnlAttribPtsComp.Visible = false;
            PnlFiche.Visible = true;
            actualisationLblFichePerso();
        }

        private void BtnAttribPtsCompPV_Click(object sender, EventArgs e)
        {
            if(Talent == "Bizarre" && PtsCompApres >= 1)
            {
                attributionPtsComp("PV", 1);
            }
            else
            {
                if(Talent != "Bizarre" && PtsCompApres >= 2)
                {
                    attributionPtsComp("PV", 2);
                }
            }
            actualisationLblAttribPtsComp();
        }

        private void BtnAttribPtsCompCrit_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && PtsCompApres >= 1)
            {
                attributionPtsComp("Crit", 1);
            }
            else
            {
                if(Talent != "Bizarre" && PtsCompApres >= 2)
                {
                    attributionPtsComp("Crit", 2);
                }
            }
            actualisationLblAttribPtsComp();
        }

        private void BtnAttribPtsCompPre_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && PtsCompApres >= 1)
            {
                attributionPtsComp("Pre", 1);
            }
            else
            {
                if(Talent != "Bizarre" && PtsCompApres >= 2)
                {
                    attributionPtsComp("Pre", 2);
                }
            }
            actualisationLblAttribPtsComp();
        }

        private void BtnAttribPtsCompEsq_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && PtsCompApres >= 1)
            {
                attributionPtsComp("Esq", 1);
            }
            else
            {
                if(Talent != "Bizarre" && PtsCompApres >= 2)
                {
                    attributionPtsComp("Esq", 2);
                }
            }
            actualisationLblAttribPtsComp();
        }

        private void BtnAttribPtsCompVit_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && PtsCompApres >= 2)
            {
                attributionPtsComp("Vit", 2);
            }
            else
            {
                if (Talent != "Bizarre" && PtsCompApres >= 1)
                {
                    attributionPtsComp("Vit", 1);
                }
            }
            actualisationLblAttribPtsComp();
        }

        private void BtnAttribPtsCompCons_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && PtsCompApres >= 2)
            {
                attributionPtsComp("Cons", 2);
            }
            else
            {
                if (Talent != "Bizarre" && PtsCompApres >= 1)
                {
                    attributionPtsComp("Cons", 1);
                }
            }
            actualisationLblAttribPtsComp();
        }

        private void BtnAttribPtsCompInt_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && PtsCompApres >= 2)
            {
                attributionPtsComp("Int", 2);
            }
            else
            {
                if (Talent != "Bizarre" && PtsCompApres >= 1)
                {
                    attributionPtsComp("Int", 1);
                }
            }
            actualisationLblAttribPtsComp();
        }

        private void BtnAttribPtsCompCha_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && PtsCompApres >= 2)
            {
                attributionPtsComp("Cha", 2);
            }
            else
            {
                if (Talent != "Bizarre" && PtsCompApres >= 1)
                {
                    attributionPtsComp("Cha", 1);
                }
            }
            actualisationLblAttribPtsComp();
        }

        private void BtnAttribPtsCompSau_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && PtsCompApres >= 2)
            {
                attributionPtsComp("Sau", 2);
            }
            else
            {
                if (Talent != "Bizarre" && PtsCompApres >= 1)
                {
                    attributionPtsComp("Sau", 1);
                }
            }
            actualisationLblAttribPtsComp();
        }

        private void BtnAttribPtsCompIns_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && PtsCompApres >= 2)
            {
                attributionPtsComp("Ins", 2);
            }
            else
            {
                if (Talent != "Bizarre" && PtsCompApres >= 1)
                {
                    attributionPtsComp("Ins", 1);
                }
            }
            actualisationLblAttribPtsComp();
        }

        //Panel Objets Comp

        private void BtnObjCompObjets_Click(object sender, EventArgs e)
        {
            
        }

        private void BtnObjCompCompetences_Click(object sender, EventArgs e)
        {
            PnlComp.Location = new Point(0, 0);
            PnlComp.Visible = true;
            PnlObjComp.Visible = false;

            comp_selectionee.Nom = "";
            apparitionBoutonsComp();
        }

        private void BtnObjCompRetour_Click(object sender, EventArgs e)
        {
            PnlObjComp.Visible = false;
            PnlFiche.Visible = true;
            actualisationLblFichePerso();
        }

        //Panel Comp

        private void BtnCompValider_Click(object sender, EventArgs e)
        {
            if(comp_selectionee.Nom != "")
            {

                actualisationLblFichePerso();
            }
        }

        private void BtnCompRetour_Click(object sender, EventArgs e)
        {
            PnlComp.Visible = false;
            PnlFiche.Visible = true;
            actualisationLblFichePerso();
        }

        private void BtnComp1_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[0];
            TxtBoxComp.Text = "Compétence sélectionnée : " + comp_selectionee.Nom;
        }

        private void BtnComp2_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[1];
            TxtBoxComp.Text = "Compétence sélectionnée : " + comp_selectionee.Nom;
        }

        private void BtnComp3_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[2];
            TxtBoxComp.Text = "Compétence sélectionnée : " + comp_selectionee.Nom;
        }

        private void BtnComp4_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[3];
            TxtBoxComp.Text = "Compétence sélectionnée : " + comp_selectionee.Nom;
        }

        private void BtnComp5_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[4];
            TxtBoxComp.Text = "Compétence sélectionnée : " + comp_selectionee.Nom;
        }

        private void BtnComp6_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[5];
            TxtBoxComp.Text = "Compétence sélectionnée : " + comp_selectionee.Nom;
        }

        private void BtnComp7_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[6];
            TxtBoxComp.Text = "Compétence sélectionnée : " + comp_selectionee.Nom;
        }

        private void BtnComp8_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[7];
            TxtBoxComp.Text = "Compétence sélectionnée : " + comp_selectionee.Nom;
        }

        //Panel Comp Alchimie 1

        private void BtnCompAlchimieObj1_Click(object sender, EventArgs e)
        {
            if(comp_alchimie_obj1 != "")
            {
                comp_alchimie_obj1 = InventaireJoueur[0].Nom;
                comp_alchimie_qte_1 = InventaireJoueur[0].Quantité;
                comp_alchimie_rarete = InventaireJoueur[0].Rarete;
                apparitionBtnCompAlchimieRarete(ref InventaireJoueur[0].Rarete);
            }
            else
            {
                if (comp_alchimie_obj2 != "")
                {
                    comp_alchimie_obj2 = InventaireJoueur[0].Nom;
                    comp_alchimie_qte_2 = InventaireJoueur[0].Quantité;
                }
            }

            actualisationTxtBoxCompAlchimie();
        }

        private void BtnCompAlchimieObj2_Click(object sender, EventArgs e)
        {
            if(flag_comp_alchimie)
            {
                comp_alchimie_chx_objet.Nom = "";
                PnlCompAlchimie2.Location = new Point(0, 0);
                PnlCompAlchimie2.Visible = true;
                PnlCompAlchimie1.Visible = false;

                int i = 0;

                while (comp_alchimie_obj1 != InventaireJoueur[i].Nom)
                {
                    i++;
                }
                InventaireJoueur[i].Type = "";
                InventaireJoueur[i].Nom = "";
                InventaireJoueur[i].Rarete = "";
                InventaireJoueur[i].Portee = "";
                InventaireJoueur[i].BonusType = "";
                InventaireJoueur[i].Bonus = 0;
                InventaireJoueur[i].EffetType1 = "";
                InventaireJoueur[i].EffetDes1 = "";
                InventaireJoueur[i].EffetType2 = "";
                InventaireJoueur[i].EffetDes2 = "";
                InventaireJoueur[i].EffetType3 = "";
                InventaireJoueur[i].EffetDes3 = "";
                InventaireJoueur[i].Quantité = 0;
                InventaireJoueur[i].Prix = 0;
                InventaireJoueur[i].EmplOccupe = false;

                i = 0;

                while (comp_alchimie_obj2 != InventaireJoueur[i].Nom)
                {
                    i++;
                }
                InventaireJoueur[i].Type = "";
                InventaireJoueur[i].Nom = "";
                InventaireJoueur[i].Rarete = "";
                InventaireJoueur[i].Portee = "";
                InventaireJoueur[i].BonusType = "";
                InventaireJoueur[i].Bonus = 0;
                InventaireJoueur[i].EffetType1 = "";
                InventaireJoueur[i].EffetDes1 = "";
                InventaireJoueur[i].EffetType2 = "";
                InventaireJoueur[i].EffetDes2 = "";
                InventaireJoueur[i].EffetType3 = "";
                InventaireJoueur[i].EffetDes3 = "";
                InventaireJoueur[i].Quantité = 0;
                InventaireJoueur[i].Prix = 0;
                InventaireJoueur[i].EmplOccupe = false;
            }
            else
            {
                if (comp_alchimie_obj1 != "")
                {
                    comp_alchimie_obj1 = InventaireJoueur[1].Nom;
                    comp_alchimie_qte_1 = InventaireJoueur[1].Quantité;
                    comp_alchimie_rarete = InventaireJoueur[1].Rarete;
                    apparitionBtnCompAlchimieRarete(ref InventaireJoueur[1].Rarete);
                }
                else
                {
                    if (comp_alchimie_obj2 != "")
                    {
                        comp_alchimie_obj2 = InventaireJoueur[1].Nom;
                        comp_alchimie_qte_2 = InventaireJoueur[1].Quantité;
                    }
                }

                actualisationTxtBoxCompAlchimie();
            }
        }

        private void BtnCompAlchimieObj3_Click(object sender, EventArgs e)
        {
            if(flag_comp_alchimie)
            {
                int i = 0;

                while(comp_alchimie_obj1 != InventaireJoueur[i].Nom)
                {
                    i++;
                }
                InventaireJoueur[i].Type = "";
                InventaireJoueur[i].Nom = "";
                InventaireJoueur[i].Rarete = "";
                InventaireJoueur[i].Portee = "";
                InventaireJoueur[i].BonusType = "";
                InventaireJoueur[i].Bonus = 0;
                InventaireJoueur[i].EffetType1 = "";
                InventaireJoueur[i].EffetDes1 = "";
                InventaireJoueur[i].EffetType2 = "";
                InventaireJoueur[i].EffetDes2 = "";
                InventaireJoueur[i].EffetType3 = "";
                InventaireJoueur[i].EffetDes3 = "";
                InventaireJoueur[i].Quantité = 0;
                InventaireJoueur[i].Prix = 0;
                InventaireJoueur[i].EmplOccupe = false;

                i = 0;

                while (comp_alchimie_obj2 != InventaireJoueur[i].Nom)
                {
                    i++;
                }
                InventaireJoueur[i].Type = "";
                InventaireJoueur[i].Nom = "";
                InventaireJoueur[i].Rarete = "";
                InventaireJoueur[i].Portee = "";
                InventaireJoueur[i].BonusType = "";
                InventaireJoueur[i].Bonus = 0;
                InventaireJoueur[i].EffetType1 = "";
                InventaireJoueur[i].EffetDes1 = "";
                InventaireJoueur[i].EffetType2 = "";
                InventaireJoueur[i].EffetDes2 = "";
                InventaireJoueur[i].EffetType3 = "";
                InventaireJoueur[i].EffetDes3 = "";
                InventaireJoueur[i].Quantité = 0;
                InventaireJoueur[i].Prix = 0;
                InventaireJoueur[i].EmplOccupe = false;

                flag_comp_alchimie = false;

                PnlCompAlchimie1.Visible = false;
                PnlFiche.Visible = true;
                actualisationLblFichePerso();
            }
            else
            {
                if (comp_alchimie_obj1 != "")
                {
                    comp_alchimie_obj1 = InventaireJoueur[2].Nom;
                    comp_alchimie_qte_1 = InventaireJoueur[2].Quantité;
                    comp_alchimie_rarete = InventaireJoueur[2].Rarete;
                    apparitionBtnCompAlchimieRarete(ref InventaireJoueur[2].Rarete);
                }
                else
                {
                    if (comp_alchimie_obj2 != "")
                    {
                        comp_alchimie_obj2 = InventaireJoueur[2].Nom;
                        comp_alchimie_qte_2 = InventaireJoueur[2].Quantité;
                    }
                }

                actualisationTxtBoxCompAlchimie();
            }
        }

        private void BtnCompAlchimieObj4_Click(object sender, EventArgs e)
        {
            if (comp_alchimie_obj1 != "")
            {
                comp_alchimie_obj1 = InventaireJoueur[3].Nom;
                comp_alchimie_qte_1 = InventaireJoueur[3].Quantité;
                comp_alchimie_rarete = InventaireJoueur[3].Rarete;
                apparitionBtnCompAlchimieRarete(ref InventaireJoueur[3].Rarete);
            }
            else
            {
                if (comp_alchimie_obj2 != "")
                {
                    comp_alchimie_obj2 = InventaireJoueur[3].Nom;
                    comp_alchimie_qte_2 = InventaireJoueur[3].Quantité;
                }
            }

            actualisationTxtBoxCompAlchimie();
        }

        private void BtnCompAlchimieObj5_Click(object sender, EventArgs e)
        {
            if (comp_alchimie_obj1 != "")
            {
                comp_alchimie_obj1 = InventaireJoueur[4].Nom;
                comp_alchimie_qte_1 = InventaireJoueur[4].Quantité;
                comp_alchimie_rarete = InventaireJoueur[4].Rarete;
                apparitionBtnCompAlchimieRarete(ref InventaireJoueur[4].Rarete);
            }
            else
            {
                if (comp_alchimie_obj2 != "")
                {
                    comp_alchimie_obj2 = InventaireJoueur[4].Nom;
                    comp_alchimie_qte_2 = InventaireJoueur[4].Quantité;
                }
            }

            actualisationTxtBoxCompAlchimie();
        }

        private void BtnCompAlchimieObj6_Click(object sender, EventArgs e)
        {
            if (comp_alchimie_obj1 != "")
            {
                comp_alchimie_obj1 = InventaireJoueur[5].Nom;
                comp_alchimie_qte_1 = InventaireJoueur[5].Quantité;
                comp_alchimie_rarete = InventaireJoueur[5].Rarete;
                apparitionBtnCompAlchimieRarete(ref InventaireJoueur[5].Rarete);
            }
            else
            {
                if (comp_alchimie_obj2 != "")
                {
                    comp_alchimie_obj2 = InventaireJoueur[5].Nom;
                    comp_alchimie_qte_2 = InventaireJoueur[5].Quantité;
                }
            }

            actualisationTxtBoxCompAlchimie();
        }

        private void BtnCompAlchimieObj7_Click(object sender, EventArgs e)
        {
            if (comp_alchimie_obj1 != "")
            {
                comp_alchimie_obj1 = InventaireJoueur[6].Nom;
                comp_alchimie_qte_1 = InventaireJoueur[6].Quantité;
                comp_alchimie_rarete = InventaireJoueur[6].Rarete;
                apparitionBtnCompAlchimieRarete(ref InventaireJoueur[6].Rarete);
            }
            else
            {
                if (comp_alchimie_obj2 != "")
                {
                    comp_alchimie_obj2 = InventaireJoueur[6].Nom;
                    comp_alchimie_qte_2 = InventaireJoueur[6].Quantité;
                }
            }

            actualisationTxtBoxCompAlchimie();
        }

        private void BtnCompAlchimieObj8_Click(object sender, EventArgs e)
        {
            if (comp_alchimie_obj1 != "")
            {
                comp_alchimie_obj1 = InventaireJoueur[7].Nom;
                comp_alchimie_qte_1 = InventaireJoueur[7].Quantité;
                comp_alchimie_rarete = InventaireJoueur[7].Rarete;
                apparitionBtnCompAlchimieRarete(ref InventaireJoueur[7].Rarete);
            }
            else
            {
                if (comp_alchimie_obj2 != "")
                {
                    comp_alchimie_obj2 = InventaireJoueur[7].Nom;
                    comp_alchimie_qte_2 = InventaireJoueur[7].Quantité;
                }
            }

            actualisationTxtBoxCompAlchimie();
        }

        private void BtnCompAlchimieRetour_Click(object sender, EventArgs e)
        {
            PnlCompAlchimie1.Visible = false;
            PnlComp.Visible = true;
        }

        private void BtnCompAlchimieReinitialiser_Click(object sender, EventArgs e)
        {
            comp_alchimie_obj1 = "";
            comp_alchimie_obj2 = "";

            actualisationTxtBoxCompAlchimie();
            apparitionBtnCompAlchimieType();
        }

        private void BtnCompAlchimieValider_Click(object sender, EventArgs e)
        {
            if(comp_alchimie_obj1 != "" && comp_alchimie_obj2 != "")
            {
                BtnCompAlchimieRetour.Visible = false;
                BtnCompAlchimieReinitialiser.Visible = false;
                BtnCompAlchimieValider.Visible = false;
                BtnCompAlchimieObj1.Visible = false;
                BtnCompAlchimieObj4.Visible = false;
                BtnCompAlchimieObj5.Visible = false;
                BtnCompAlchimieObj6.Visible = false;
                BtnCompAlchimieObj7.Visible = false;
                BtnCompAlchimieObj8.Visible = false;
                TxtBoxCompAlchimieChx1.Visible = false;
                TxtBoxCompAlchimieChx2.Visible = false;
                LblCompAlchimieTitre.Text = "";

                BtnCompAlchimieObj2.Visible = true;
                BtnCompAlchimieObj3.Visible = true;
                BtnCompAlchimieObj2.Text = "Réussite";
                BtnCompAlchimieObj3.Text = "Echec";
                flag_comp_alchimie = true;
                comp_alchimie_qte_tot = comp_alchimie_qte_1 + comp_alchimie_qte_2;
            }
        }

        //Panel Comp Alchimie 2

        private void BtnCompAlchimie2Soin_Click(object sender, EventArgs e)
        {
            comp_alchimie_type = "Soin";
            actualisationObjetsAlchimie(ref ObjetsAlchimie, ref Inventaire, comp_alchimie_type, comp_alchimie_rarete);

            PnlCompAlchimie3.Location = new Point(0, 0);
            PnlCompAlchimie3.Visible = true;
            PnlCompAlchimie2.Visible = false;
        }

        private void BtnCompAlchimie2Degats_Click(object sender, EventArgs e)
        {
            comp_alchimie_type = "Degats";
            actualisationObjetsAlchimie(ref ObjetsAlchimie, ref Inventaire, comp_alchimie_type, comp_alchimie_rarete);

            PnlCompAlchimie3.Location = new Point(0, 0);
            PnlCompAlchimie3.Visible = true;
            PnlCompAlchimie2.Visible = false;
        }

        private void BtnCompAlchimie2Buff_Click(object sender, EventArgs e)
        {
            comp_alchimie_type = "Buff";
            actualisationObjetsAlchimie(ref ObjetsAlchimie, ref Inventaire, comp_alchimie_type, comp_alchimie_rarete);

            PnlCompAlchimie3.Location = new Point(0, 0);
            PnlCompAlchimie3.Visible = true;
            PnlCompAlchimie2.Visible = false;
        }

        private void BtnCompAlchimie2Debuff_Click(object sender, EventArgs e)
        {
            comp_alchimie_type = "Debuff";
            actualisationObjetsAlchimie(ref ObjetsAlchimie, ref Inventaire, comp_alchimie_type, comp_alchimie_rarete);

            PnlCompAlchimie3.Location = new Point(0, 0);
            PnlCompAlchimie3.Visible = true;
            PnlCompAlchimie2.Visible = false;
        }

        //Panel Comp Alchimie 3

        private void BtnCompAlchimie3Obj1_Click(object sender, EventArgs e)
        {
            comp_alchimie_chx_objet = ObjetsAlchimie[0];
            TxtBoxCompAlchimie3.Text = "Objet selectionné : " + comp_alchimie_chx_objet.Nom;
        }

        private void BtnCompAlchimie3Obj2_Click(object sender, EventArgs e)
        {
            comp_alchimie_chx_objet = ObjetsAlchimie[1];
            TxtBoxCompAlchimie3.Text = "Objet selectionné : " + comp_alchimie_chx_objet.Nom;
        }

        private void BtnCompAlchimie3Obj3_Click(object sender, EventArgs e)
        {
            comp_alchimie_chx_objet = ObjetsAlchimie[2];
            TxtBoxCompAlchimie3.Text = "Objet selectionné : " + comp_alchimie_chx_objet.Nom;
        }

        private void BtnCompAlchimie3Obj4_Click(object sender, EventArgs e)
        {
            comp_alchimie_chx_objet = ObjetsAlchimie[3];
            TxtBoxCompAlchimie3.Text = "Objet selectionné : " + comp_alchimie_chx_objet.Nom;
        }

        private void BtnCompAlchimie3Obj5_Click(object sender, EventArgs e)
        {
            comp_alchimie_chx_objet = ObjetsAlchimie[4];
            TxtBoxCompAlchimie3.Text = "Objet selectionné : " + comp_alchimie_chx_objet.Nom;
        }

        private void BtnCompAlchimie3Obj6_Click(object sender, EventArgs e)
        {
            comp_alchimie_chx_objet = ObjetsAlchimie[5];
            TxtBoxCompAlchimie3.Text = "Objet selectionné : " + comp_alchimie_chx_objet.Nom;
        }

        private void BtnCompAlchimie3Retour_Click(object sender, EventArgs e)
        {
            PnlCompAlchimie3.Visible = false;
            PnlCompAlchimie2.Visible = true;
        }

        private void BtnCompAlchimie3Valider_Click(object sender, EventArgs e)
        {
            if(comp_alchimie_chx_objet.Nom != "")
            {
                ajoutObjetAlchimie(ref comp_alchimie_qte_tot, ref InventaireJoueur, ref comp_alchimie_chx_objet);
                comp_alchimie_obj1 = "";
                comp_alchimie_obj2 = "";

                PnlCompAlchimie3.Visible = false;
                PnlFiche.Visible = true;
                actualisationLblFichePerso();
            }
        }

        //Panel Comp Custom

        private void BtnCompCustomPV_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && nb_pts_custom_apres >= 1)
            {
                attributionPtsCustom("PV", 1);
            }
            else
            {
                if (Talent != "Bizarre" && nb_pts_custom_apres >= 2)
                {
                    attributionPtsCustom("PV", 2);
                }
            }
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom_apres;
        }

        private void BtnCompCustomCrit_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && nb_pts_custom_apres >= 1)
            {
                attributionPtsCustom("Crit", 1);
            }
            else
            {
                if (Talent != "Bizarre" && nb_pts_custom_apres >= 2)
                {
                    attributionPtsCustom("Crit", 2);
                }
            }
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom_apres;
        }

        private void BtnCompCustomPre_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && nb_pts_custom_apres >= 1)
            {
                attributionPtsCustom("Pre", 1);
            }
            else
            {
                if (Talent != "Bizarre" && nb_pts_custom_apres >= 2)
                {
                    attributionPtsCustom("Pre", 2);
                }
            }
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom_apres;
        }

        private void BtnCompCustomEsq_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && nb_pts_custom_apres >= 1)
            {
                attributionPtsCustom("Esq", 1);
            }
            else
            {
                if (Talent != "Bizarre" && nb_pts_custom_apres >= 2)
                {
                    attributionPtsCustom("Esq", 2);
                }
            }
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom_apres;
        }

        private void BtnCompCustomVit_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && nb_pts_custom_apres >= 2)
            {
                attributionPtsCustom("Vit", 2);
            }
            else
            {
                if (Talent != "Bizarre" && nb_pts_custom_apres >= 1)
                {
                    attributionPtsCustom("Vit", 1);
                }
            }
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom_apres;
        }

        private void BtnCompCustomCons_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && nb_pts_custom_apres >= 2)
            {
                attributionPtsCustom("Cons", 2);
            }
            else
            {
                if (Talent != "Bizarre" && nb_pts_custom_apres >= 1)
                {
                    attributionPtsCustom("Cons", 1);
                }
            }
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom_apres;
        }

        private void BtnCompCustomInt_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && nb_pts_custom_apres >= 2)
            {
                attributionPtsCustom("Int", 2);
            }
            else
            {
                if (Talent != "Bizarre" && nb_pts_custom_apres >= 1)
                {
                    attributionPtsCustom("Int", 1);
                }
            }
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom_apres;
        }

        private void BtnCompCustomCha_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && nb_pts_custom_apres >= 2)
            {
                attributionPtsCustom("Cha", 2);
            }
            else
            {
                if (Talent != "Bizarre" && nb_pts_custom_apres >= 1)
                {
                    attributionPtsCustom("Cha", 1);
                }
            }
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom_apres;
        }

        private void BtnCompCustomSau_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && nb_pts_custom_apres >= 2)
            {
                attributionPtsCustom("Sau", 2);
            }
            else
            {
                if (Talent != "Bizarre" && nb_pts_custom_apres >= 1)
                {
                    attributionPtsCustom("Sau", 1);
                }
            }
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom_apres;
        }

        private void BtnCompCustomIns_Click(object sender, EventArgs e)
        {
            if (Talent == "Bizarre" && nb_pts_custom_apres >= 2)
            {
                attributionPtsCustom("Ins", 2);
            }
            else
            {
                if (Talent != "Bizarre" && nb_pts_custom_apres >= 1)
                {
                    attributionPtsCustom("Ins", 1);
                }
            }
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom_apres;
        }

        private void BtnCompCustomReinitialiser_Click(object sender, EventArgs e)
        {
            ptPV = 0;
            ptPre = 0;
            ptEsq = 0;
            ptCrit = 0;
            ptVit = 0;
            ptCons = 0;
            ptInt = 0;
            ptCha = 0;
            ptSau = 0;
            ptIns = 0;
            nb_pts_custom_apres = nb_pts_custom;
            TxtBoxCompCustom.Text = "";
            LblCompCustomPtsRestants.Text = "Points restants : " + nb_pts_custom.ToString();
        }

        private void BtnCompCustomValider_Click(object sender, EventArgs e)
        {
            if(nb_pts_custom_apres == 0)
            {
                int i = 0;

                while(i < 8 && CompJoueur[i].Nom != "")
                {
                    i++;
                }

                new_comp.Type = "Passive";
                new_comp.Effet = TxtBoxCompCustom.Text;
                new_comp.BonusPV = ptPV;
                new_comp.BonusPre = ptPre;
                new_comp.BonusEsq = ptEsq;
                new_comp.BonusCrit = ptCrit;
                new_comp.BonusVit = ptVit;
                new_comp.BonusCons = ptCons;
                new_comp.BonusInt = ptInt;
                new_comp.BonusCha = ptCha;
                new_comp.BonusSau = ptSau;
                new_comp.BonusIns = ptIns;

                switch (nb_pts_custom)
                {
                    case 3:
                        new_comp.Nom = "Custom A (" + TxtBoxCompCustom.Text + ")";
                        break;
                    case 5:
                        new_comp.Nom = "Custom B (" + TxtBoxCompCustom.Text + ")";
                        break;
                    case 10:
                        new_comp.Nom = "Custom C (" + TxtBoxCompCustom.Text + ")";
                        break;
                    default:
                        new_comp.Nom = "Custom Z (" + TxtBoxCompCustom.Text + ")";
                        break;
                }

                if (i == 8)
                {
                    actualisationPnlRemplacementComp();
                    PnlCompCustom.Visible = false;
                    PnlRemplacementComp.Location = new Point(0, 0);
                    PnlRemplacementComp.Visible = true;
                }
                else
                {
                    CompJoueur[i] = new_comp;

                    PnlCompCustom.Visible = false;
                    PnlMarchéAchat.Visible = true;
                    TxtBoxCompCustom.Text = "";
                    calculStatsTot();
                    actualisationLblFondsMarché();
                    actualisationLblFichePerso();
                }
            }
        }

        //Panel Remplacement Comp

        private void BtnRemplacementComp1_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[0];
            TxtBoxRemplacementComp.Text = "Compétence à oublier : " + comp_selectionee.Nom;
        }

        private void BtnRemplacementComp2_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[1];
            TxtBoxRemplacementComp.Text = "Compétence à oublier : " + comp_selectionee.Nom;
        }

        private void BtnRemplacementComp3_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[2];
            TxtBoxRemplacementComp.Text = "Compétence à oublier : " + comp_selectionee.Nom;
        }

        private void BtnRemplacementComp4_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[3];
            TxtBoxRemplacementComp.Text = "Compétence à oublier : " + comp_selectionee.Nom;
        }

        private void BtnRemplacementComp5_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[4];
            TxtBoxRemplacementComp.Text = "Compétence à oublier : " + comp_selectionee.Nom;
        }

        private void BtnRemplacementComp6_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[5];
            TxtBoxRemplacementComp.Text = "Compétence à oublier : " + comp_selectionee.Nom;
        }

        private void BtnRemplacementComp7_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[6];
            TxtBoxRemplacementComp.Text = "Compétence à oublier : " + comp_selectionee.Nom;
        }

        private void BtnRemplacementComp8_Click(object sender, EventArgs e)
        {
            comp_selectionee = CompJoueur[7];
            TxtBoxRemplacementComp.Text = "Compétence à oublier : " + comp_selectionee.Nom;
        }

        private void BtnRemplacementCompSup_Click(object sender, EventArgs e)
        {
            comp_selectionee = new_comp;
            TxtBoxRemplacementComp.Text = "Compétence à oublier : " + comp_selectionee.Nom;
        }

        private void BtnRemplacementCompRemplacer_Click(object sender, EventArgs e)
        {
            if(comp_selectionee.Nom != "")
            {
                if(comp_selectionee.Nom != new_comp.Nom)
                {
                    if(comp_selectionee.Nom == CompJoueur[0].Nom)
                    {
                        CompJoueur[0] = new_comp;
                    }
                    else
                    {
                        if (comp_selectionee.Nom == CompJoueur[1].Nom)
                        {
                            CompJoueur[1] = new_comp;
                        }
                        else
                        {
                            if (comp_selectionee.Nom == CompJoueur[2].Nom)
                            {
                                CompJoueur[2] = new_comp;
                            }
                            else
                            {
                                if (comp_selectionee.Nom == CompJoueur[3].Nom)
                                {
                                    CompJoueur[3] = new_comp;
                                }
                                else
                                {
                                    if (comp_selectionee.Nom == CompJoueur[4].Nom)
                                    {
                                        CompJoueur[4] = new_comp;
                                    }
                                    else
                                    {
                                        if (comp_selectionee.Nom == CompJoueur[5].Nom)
                                        {
                                            CompJoueur[5] = new_comp;
                                        }
                                        else
                                        {
                                            if (comp_selectionee.Nom == CompJoueur[6].Nom)
                                            {
                                                CompJoueur[6] = new_comp;
                                            }
                                            else
                                            {
                                                CompJoueur[7] = new_comp;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                PnlRemplacementComp.Visible = false;
                actualisationLblFondsMarché();
                actualisationLblFichePerso();

                if (comp_marche)
                {
                    PnlMarchéAchat.Visible = true;
                }
                else
                {
                    PnlFiche.Visible = true;
                }
            }
        }

        //Fonction changement Niveau

        public void chgtNiveau()
        {
            comp_marche = false;

            if(ClassePerso.ExpAct >= ClassePerso.ExpSup)
            {
                ClassePerso.ExpAct -= ClassePerso.ExpSup;
                ClassePerso.ExpSup += 10;
                ClassePerso.Niv++;
                PtsComp += 3;

                if (Talent == "Travailleur")
                    PtsComp++;
            }
            actualisationLblFichePerso();
        }

        //Fonction gain Exp Combat

        public void gainExpCombat()
        {
            chgtNiveau();
        }

        //Fonction gain Exp Entrainement

        public void gainExpEntrainement()
        {
            chgtNiveau();
        }

        //Fonction attribution Pts Comp

        public void attributionPtsComp(string type_pt, int nb_pt_utilises)
        {
            switch(type_pt)
            {
                case "PV":
                    ptPV++;
                    break;
                case "Pre":
                    ptPre++;
                    break;
                case "Esq":
                    ptEsq++;
                    break;
                case "Crit":
                    ptCrit++;
                    break;
                case "Vit":
                    ptVit++;
                    break;
                case "Cons":
                    ptCons++; 
                    break;
                case "Int":
                    ptInt++;
                    break;
                case "Cha":
                    ptCha++;
                    break;
                case "Sau":
                    ptSau++;
                    break;
                default:
                    ptIns++;
                    break;
            }

            PtsCompApres -= nb_pt_utilises;

            TxtBoxAttribPtsComp.Text = "";

            if(ptPV != 0)
            {
                TxtBoxAttribPtsComp.Text += "PV +" + ptPV.ToString();
            }

            if(ptPre != 0 && ptPV != 0)
            {
                TxtBoxAttribPtsComp.Text += "; Pré +" + ptPre.ToString();
            }
            else
            {
                if(ptPre != 0 && ptPV == 0)
                {
                    TxtBoxAttribPtsComp.Text += "Pré +" + ptPre.ToString();
                }
            }

            if(ptEsq != 0 && (ptPV != 0 || ptPre != 0))
            {
                TxtBoxAttribPtsComp.Text += "; Esq +" + ptEsq.ToString();
            }
            else
            {
                if(ptEsq != 0 && ptPV == 0 && ptPre == 0)
                {
                    TxtBoxAttribPtsComp.Text += "Esq +" + ptEsq.ToString();
                }
            }

            if (ptCrit != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0))
            {
                TxtBoxAttribPtsComp.Text += "; Crit +" + ptCrit.ToString();
            }
            else
            {
                if (ptCrit != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0)
                {
                    TxtBoxAttribPtsComp.Text += "Crit +" + ptCrit.ToString();
                }
            }

            if (ptVit != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0))
            {
                TxtBoxAttribPtsComp.Text += "; Vit +" + ptVit.ToString();
            }
            else
            {
                if (ptVit != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0)
                {
                    TxtBoxAttribPtsComp.Text += "Vit +" + ptVit.ToString();
                }
            }

            if (ptCons != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0 || ptVit != 0))
            {
                TxtBoxAttribPtsComp.Text += "; Cons +" + ptCons.ToString();
            }
            else
            {
                if (ptCons != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0 && ptVit == 0)
                {
                    TxtBoxAttribPtsComp.Text += "Cons +" + ptCons.ToString();
                }
            }

            if (ptInt != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0 || ptVit != 0 || ptCons != 0))
            {
                TxtBoxAttribPtsComp.Text += "; Int +" + ptInt.ToString();
            }
            else
            {
                if (ptInt != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0 && ptVit == 0 && ptCons == 0)
                {
                    TxtBoxAttribPtsComp.Text += "Int +" + ptInt.ToString();
                }
            }

            if (ptCha != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0 || ptVit != 0 || ptCons != 0 || ptInt != 0))
            {
                TxtBoxAttribPtsComp.Text += "; Cha +" + ptCha.ToString();
            }
            else
            {
                if (ptCha != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0 && ptVit == 0 && ptCons == 0 && ptInt == 0)
                {
                    TxtBoxAttribPtsComp.Text += "Cha +" + ptCha.ToString();
                }
            }

            if (ptSau != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0 || ptVit != 0 || ptCons != 0 || ptInt != 0 || ptCha != 0))
            {
                TxtBoxAttribPtsComp.Text += "; Sau +" + ptSau.ToString();
            }
            else
            {
                if (ptSau != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0 && ptVit == 0 && ptCons == 0 && ptInt == 0 && ptCha == 0)
                {
                    TxtBoxAttribPtsComp.Text += "Sau +" + ptSau.ToString();
                }
            }

            if (ptIns != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0 || ptVit != 0 || ptCons != 0 || ptInt != 0 || ptCha != 0 || ptSau != 0))
            {
                TxtBoxAttribPtsComp.Text += "; Ins +" + ptIns.ToString();
            }
            else
            {
                if (ptIns != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0 && ptVit == 0 && ptCons == 0 && ptInt == 0 && ptCha == 0 && ptSau == 0)
                {
                    TxtBoxAttribPtsComp.Text += "Ins +" + ptIns.ToString();
                }
            }
        }

        //Fonction calcul Multiplicateur Argent

        public void calculMultiplicateurArgent()
        {
            MultiplicateurArgent = 1;

            if (Talent == "Chanceux")
                MultiplicateurArgent += 0.5;

            if (InventaireJoueur[0].Nom == "Picsou" || InventaireJoueur[1].Nom == "Picsou" || InventaireJoueur[2].Nom == "Picsou" || InventaireJoueur[3].Nom == "Picsou" || InventaireJoueur[4].Nom == "Picsou" || InventaireJoueur[5].Nom == "Picsou" || InventaireJoueur[6].Nom == "Picsou" || InventaireJoueur[7].Nom == "Picsou")
            {
                MultiplicateurArgent++;
            }
            else
            {
                if (InventaireJoueur[0].Nom == "Cash" || InventaireJoueur[1].Nom == "Cash" || InventaireJoueur[2].Nom == "Cash" || InventaireJoueur[3].Nom == "Cash" || InventaireJoueur[4].Nom == "Cash" || InventaireJoueur[5].Nom == "Cash" || InventaireJoueur[6].Nom == "Cash" || InventaireJoueur[7].Nom == "Cash" || InventaireJoueur[0].Nom == "Donald" || InventaireJoueur[1].Nom == "Donald" || InventaireJoueur[2].Nom == "Donald" || InventaireJoueur[3].Nom == "Donald" || InventaireJoueur[4].Nom == "Donald" || InventaireJoueur[5].Nom == "Donald" || InventaireJoueur[6].Nom == "Donald" || InventaireJoueur[7].Nom == "Donald")
                    MultiplicateurArgent += 0.5;
            }

            if (CompJoueur[0].Nom == "Plein aux as" || CompJoueur[1].Nom == "Plein aux as" || CompJoueur[2].Nom == "Plein aux as" || CompJoueur[3].Nom == "Plein aux as" || CompJoueur[4].Nom == "Plein aux as" || CompJoueur[5].Nom == "Plein aux as" || CompJoueur[6].Nom == "Plein aux as" || CompJoueur[7].Nom == "Plein aux as")
            {
                MultiplicateurArgent += 0.5;
            }

            if(CompJoueur[0].Nom == "Ingeniosite" || CompJoueur[1].Nom == "Ingeniosite" || CompJoueur[2].Nom == "Ingeniosite" || CompJoueur[3].Nom == "Ingeniosite" || CompJoueur[4].Nom == "Ingeniosite" || CompJoueur[5].Nom == "Ingeniosite" || CompJoueur[6].Nom == "Ingeniosite" || CompJoueur[7].Nom == "Ingeniosite")
            {
                if(tour_combat < 10)
                {
                    MultiplicateurArgent += 1 - 0.1 * tour_combat;
                }
            }
        }

        //Fonction actualisation Label Attribution Pts Comp

        public void actualisationLblAttribPtsComp()
        {
            LblAttribPtsCompPtsComp.Text = "Points de compétences actuels :  " + PtsCompApres.ToString();

            if (Talent == "Bizarre")
            {
                LblAttribPtsCompReqPV.Text = "Requis : 1";
                LblAttribPtsCompReqVit.Text = "Requis : 2";
            }
            else
            {
                LblAttribPtsCompReqPV.Text = "Requis : 2";
                LblAttribPtsCompReqVit.Text = "Requis : 1";
            }
        }

        //Fonction actualisation Label Fiche Perso

        public void actualisationLblFichePerso()
        {
            LblChapitre.Text = Chapitre.ToString();
            LblPhase.Text = Phase;
            LblJour.Text = Date.ToString() + ", " + MomentJournée;
            LblNom.Text = Nom;
            LblDégâts.Text = ClassePerso.Dégâts;
            LblFonds.Text = Fonds.ToString();
            LblExpAct.Text = ClassePerso.ExpAct.ToString();
            LblExpSuiv.Text = ClassePerso.ExpSup.ToString();
            LblPtsComp.Text = PtsComp.ToString();
            LblClasse.Text = ClassePerso.NomClasse;
            LblTalent.Text = Talent;
            LblStatut.Text = Statut;
            LblAge.Text = Age;
            LblNiv.Text = ClassePerso.Niv.ToString();

            LblEquipable1.Text = ClassePerso.Equipable1;
            LblEquipable2.Text = ClassePerso.Equipable2;
            LblEquipable3.Text = ClassePerso.Equipable3;
            LblEquipable4.Text = ClassePerso.Equipable4;
            LblEquipable5.Text = ClassePerso.Equipable5;
            LblEquipable6.Text = ClassePerso.Equipable6;

            LblComp1.Text = CompJoueur[0].Nom;
            LblComp2.Text = CompJoueur[1].Nom;
            LblComp3.Text = CompJoueur[2].Nom;
            LblComp4.Text = CompJoueur[3].Nom;
            LblComp5.Text = CompJoueur[4].Nom;
            LblComp6.Text = CompJoueur[5].Nom;
            LblComp7.Text = CompJoueur[6].Nom;
            LblComp8.Text = CompJoueur[7].Nom;

            LblPVBase.Text = ClassePerso.PVBase.ToString();
            LblMvtBase.Text = ClassePerso.MvtBase;
            LblSocialBase.Text = ClassePerso.SocialBase.ToString();
            LblPréBase.Text = ClassePerso.PreBase.ToString();
            LblEsqBase.Text = ClassePerso.EsqBase.ToString();
            LblCritBase.Text = ClassePerso.CritBase.ToString();
            LblVitBase.Text = ClassePerso.VitBase.ToString();
            LblConsBase.Text = ClassePerso.ConsBase.ToString();
            LblIntBase.Text = ClassePerso.IntBase.ToString();
            LblChaBase.Text = ClassePerso.ChaBase.ToString();
            LblSauBase.Text = ClassePerso.SauBase.ToString();
            LblInsBase.Text = ClassePerso.InsBase.ToString();

            LblPVEquip.Text = ClassePerso.PVEquip.ToString();
            LblPréEquip.Text = ClassePerso.PreEquip.ToString();
            LblEsqEquip.Text = ClassePerso.EsqEquip.ToString();
            LblCritEquip.Text = ClassePerso.CritEquip.ToString();
            LblVitEquip.Text = ClassePerso.VitEquip.ToString();
            LblConsEquip.Text = ClassePerso.ConsEquip.ToString();
            LblIntEquip.Text = ClassePerso.IntEquip.ToString();
            LblChaEquip.Text = ClassePerso.ChaEquip.ToString();
            LblSauEquip.Text = ClassePerso.SauEquip.ToString();
            LblInsEquip.Text = ClassePerso.InsEquip.ToString();

            LblMvtBuff.Text = ClassePerso.MvtBuff.ToString();
            LblPréBuff.Text = ClassePerso.PreBuff.ToString();
            LblEsqBuff.Text = ClassePerso.EsqBuff.ToString();
            LblCritBuff.Text = ClassePerso.CritBuff.ToString();
            LblVitBuff.Text = ClassePerso.VitBuff.ToString();
            LblConsBuff.Text = ClassePerso.ConsBuff.ToString();
            LblIntBuff.Text = ClassePerso.IntBuff.ToString();
            LblChaBuff.Text = ClassePerso.ChaBuff.ToString();
            LblSauBuff.Text = ClassePerso.SauBuff.ToString();
            LblInsBuff.Text = ClassePerso.InsBuff.ToString();

            LblPVTotAct.Text = ClassePerso.PVTotAct.ToString();
            LblPVTotMax.Text = ClassePerso.PVTotMax.ToString();
            LblMvtTot.Text = ClassePerso.MvtTot;
            LblSocialTot.Text = ClassePerso.SocialTot.ToString();
            LblPréTot.Text = ClassePerso.PreTot.ToString();
            LblEsqTot.Text = ClassePerso.EsqTot.ToString();
            LblCritTot.Text = ClassePerso.CritTot.ToString();
            LblVitTot.Text = ClassePerso.VitTot.ToString();
            LblConsTot.Text = ClassePerso.ConsTot.ToString();
            LblIntTot.Text = ClassePerso.IntTot.ToString();
            LblChaTot.Text = ClassePerso.ChaTot.ToString();
            LblSauTot.Text = ClassePerso.SauTot.ToString();
            LblInsTot.Text = ClassePerso.InsTot.ToString();

            if (InventaireJoueur[0].Nom != "")
            {
                LblObj1Nom.Text = InventaireJoueur[0].Nom;
                LblObj1Type.Text = InventaireJoueur[0].Type;
                if (InventaireJoueur[0].Type == "Transcendance")
                    LblObj1Type.Text = "Transcend.";
                LblObj1Rarete.Text = InventaireJoueur[0].Rarete;
                LblObj1Portée.Text = InventaireJoueur[0].Portee;
                LblObj1Bonus.Text = InventaireJoueur[0].BonusType + " +" + InventaireJoueur[0].Bonus;

                if (InventaireJoueur[0].Type == "Soin" || InventaireJoueur[0].Type == "Degats" || InventaireJoueur[0].Type == "Buff" || InventaireJoueur[0].Type == "Debuff")
                {
                    LblObj1Nom.Text += " (" + InventaireJoueur[0].Quantité + ")";
                    LblObj1Bonus.Text = "          /";
                    LblObj1Effet.Text = InventaireJoueur[0].EffetDes1;
                }
                else
                {
                    if (InventaireJoueur[0].EffetType1 != "")
                    {
                        LblObj1Effet.Text = InventaireJoueur[0].EffetType1 + ": " + InventaireJoueur[0].EffetDes1;

                        if (InventaireJoueur[0].EffetType2 != "")
                        {
                            LblObj1Effet.Text += " ; " + InventaireJoueur[0].EffetType2 + ": " + InventaireJoueur[0].EffetDes2;

                            if (InventaireJoueur[0].EffetType3 != "")
                            {
                                LblObj1Effet.Text += " ; " + InventaireJoueur[0].EffetType3 + ": " + InventaireJoueur[0].EffetDes3;
                            }
                        }
                    }
                    else
                    {
                        LblObj1Effet.Text = "";
                    }
                }
            }
            else
            {
                LblObj1Nom.Text = "";
                LblObj1Type.Text = "";
                LblObj1Rarete.Text = "";
                LblObj1Portée.Text = "";
                LblObj1Bonus.Text = "";
                LblObj1Effet.Text = "";
            }

            if (InventaireJoueur[0].Type == "Epee" || InventaireJoueur[0].Type == "Lance" || InventaireJoueur[0].Type == "Dague" || InventaireJoueur[0].Type == "Outil" || InventaireJoueur[0].Type == "Transcendance")
            {
                LblObj1Nom.ForeColor = Color.Red;
                LblObj1Type.ForeColor = Color.Red;
                LblObj1Rarete.ForeColor = Color.Red;
                LblObj1Portée.ForeColor = Color.Red;
                LblObj1Bonus.ForeColor = Color.Red;
                LblObj1Effet.ForeColor = Color.Red;
            }
            else
            {
                if (InventaireJoueur[0].Type == "Arc" || InventaireJoueur[0].Type == "Tome" || InventaireJoueur[0].Type == "Lancers" || InventaireJoueur[0].Type == "Arme a feu" || InventaireJoueur[0].Type == "Fronde")
                {
                    LblObj1Nom.ForeColor = Color.Blue;
                    LblObj1Type.ForeColor = Color.Blue;
                    LblObj1Rarete.ForeColor = Color.Blue;
                    LblObj1Portée.ForeColor = Color.Blue;
                    LblObj1Bonus.ForeColor = Color.Blue;
                    LblObj1Effet.ForeColor = Color.Blue;
                }
                else
                {
                    if (InventaireJoueur[0].Type == "Hache" || InventaireJoueur[0].Type == "Bouclier" || InventaireJoueur[0].Type == "Poutre" || InventaireJoueur[0].Type == "Pugilats" || InventaireJoueur[0].Type == "Bestipierre")
                    {
                        LblObj1Nom.ForeColor = Color.Green;
                        LblObj1Type.ForeColor = Color.Green;
                        LblObj1Rarete.ForeColor = Color.Green;
                        LblObj1Portée.ForeColor = Color.Green;
                        LblObj1Bonus.ForeColor = Color.Green;
                        LblObj1Effet.ForeColor = Color.Green;
                    }
                    else
                    {
                        LblObj1Nom.ForeColor = Color.Black;
                        LblObj1Type.ForeColor = Color.Black;
                        LblObj1Rarete.ForeColor = Color.Black;
                        LblObj1Portée.ForeColor = Color.Black;
                        LblObj1Bonus.ForeColor = Color.Black;
                        LblObj1Effet.ForeColor = Color.Black;
                    }
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (InventaireJoueur[1].Nom != "")
            {
                LblObj2Nom.Text = InventaireJoueur[1].Nom;
                LblObj2Type.Text = InventaireJoueur[1].Type;
                if (InventaireJoueur[1].Type == "Transcendance")
                    LblObj2Type.Text = "Transcend.";
                LblObj2Rarete.Text = InventaireJoueur[1].Rarete;
                LblObj2Portée.Text = InventaireJoueur[1].Portee;
                LblObj2Bonus.Text = InventaireJoueur[1].BonusType + " +" + InventaireJoueur[1].Bonus;

                if (InventaireJoueur[1].Type == "Soin" || InventaireJoueur[1].Type == "Degats" || InventaireJoueur[1].Type == "Buff" || InventaireJoueur[1].Type == "Debuff")
                {
                    LblObj2Nom.Text += " (" + InventaireJoueur[1].Quantité + ")";
                    LblObj2Bonus.Text = "          /";
                    LblObj2Effet.Text = InventaireJoueur[1].EffetDes1;
                }
                else
                {
                    if (InventaireJoueur[1].EffetType1 != "")
                    {
                        LblObj2Effet.Text = InventaireJoueur[1].EffetType1 + ": " + InventaireJoueur[1].EffetDes1;

                        if (InventaireJoueur[1].EffetType2 != "")
                        {
                            LblObj2Effet.Text += " ; " + InventaireJoueur[1].EffetType2 + ": " + InventaireJoueur[1].EffetDes2;

                            if (InventaireJoueur[1].EffetType3 != "")
                            {
                                LblObj2Effet.Text += " ; " + InventaireJoueur[1].EffetType3 + ": " + InventaireJoueur[1].EffetDes3;
                            }
                        }
                    }
                    else
                    {
                        LblObj2Effet.Text = "";
                    }
                }
            }
            else
            {
                LblObj2Nom.Text = "";
                LblObj2Type.Text = "";
                LblObj2Rarete.Text = "";
                LblObj2Portée.Text = "";
                LblObj2Bonus.Text = "";
                LblObj2Effet.Text = "";
            }

            if (InventaireJoueur[1].Type == "Epee" || InventaireJoueur[1].Type == "Lance" || InventaireJoueur[1].Type == "Dague" || InventaireJoueur[1].Type == "Outil" || InventaireJoueur[1].Type == "Transcendance")
            {
                LblObj2Nom.ForeColor = Color.Red;
                LblObj2Type.ForeColor = Color.Red;
                LblObj2Rarete.ForeColor = Color.Red;
                LblObj2Portée.ForeColor = Color.Red;
                LblObj2Bonus.ForeColor = Color.Red;
                LblObj2Effet.ForeColor = Color.Red;
            }
            else
            {
                if (InventaireJoueur[1].Type == "Arc" || InventaireJoueur[1].Type == "Tome" || InventaireJoueur[1].Type == "Lancers" || InventaireJoueur[1].Type == "Arme a feu" || InventaireJoueur[1].Type == "Fronde")
                {
                    LblObj2Nom.ForeColor = Color.Blue;
                    LblObj2Type.ForeColor = Color.Blue;
                    LblObj2Rarete.ForeColor = Color.Blue;
                    LblObj2Portée.ForeColor = Color.Blue;
                    LblObj2Bonus.ForeColor = Color.Blue;
                    LblObj2Effet.ForeColor = Color.Blue;
                }
                else
                {
                    if (InventaireJoueur[1].Type == "Hache" || InventaireJoueur[1].Type == "Bouclier" || InventaireJoueur[1].Type == "Poutre" || InventaireJoueur[1].Type == "Pugilats" || InventaireJoueur[1].Type == "Bestipierre")
                    {
                        LblObj2Nom.ForeColor = Color.Green;
                        LblObj2Type.ForeColor = Color.Green;
                        LblObj2Rarete.ForeColor = Color.Green;
                        LblObj2Portée.ForeColor = Color.Green;
                        LblObj2Bonus.ForeColor = Color.Green;
                        LblObj2Effet.ForeColor = Color.Green;
                    }
                    else
                    {
                        LblObj2Nom.ForeColor = Color.Black;
                        LblObj2Type.ForeColor = Color.Black;
                        LblObj2Rarete.ForeColor = Color.Black;
                        LblObj2Portée.ForeColor = Color.Black;
                        LblObj2Bonus.ForeColor = Color.Black;
                        LblObj2Effet.ForeColor = Color.Black;
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (InventaireJoueur[2].Nom != "")
            {
                LblObj3Nom.Text = InventaireJoueur[2].Nom;
                LblObj3Type.Text = InventaireJoueur[2].Type;
                if (InventaireJoueur[2].Type == "Transcendance")
                    LblObj3Type.Text = "Transcend.";
                LblObj3Rarete.Text = InventaireJoueur[2].Rarete;
                LblObj3Portée.Text = InventaireJoueur[2].Portee;
                LblObj3Bonus.Text = InventaireJoueur[2].BonusType + " +" + InventaireJoueur[2].Bonus;

                if (InventaireJoueur[2].Type == "Soin" || InventaireJoueur[2].Type == "Degats" || InventaireJoueur[2].Type == "Buff" || InventaireJoueur[2].Type == "Debuff")
                {
                    LblObj3Nom.Text += " (" + InventaireJoueur[2].Quantité + ")";
                    LblObj3Bonus.Text = "          /";
                    LblObj3Effet.Text = InventaireJoueur[2].EffetDes1;
                }
                else
                {
                    if (InventaireJoueur[2].EffetType1 != "")
                    {
                        LblObj3Effet.Text = InventaireJoueur[2].EffetType1 + ": " + InventaireJoueur[2].EffetDes1;

                        if (InventaireJoueur[2].EffetType2 != "")
                        {
                            LblObj3Effet.Text += " ; " + InventaireJoueur[2].EffetType2 + ": " + InventaireJoueur[2].EffetDes2;

                            if (InventaireJoueur[2].EffetType3 != "")
                            {
                                LblObj3Effet.Text += " ; " + InventaireJoueur[2].EffetType3 + ": " + InventaireJoueur[2].EffetDes3;
                            }
                        }
                    }
                    else
                    {
                        LblObj3Effet.Text = "";
                    }
                }
            }
            else
            {
                LblObj3Nom.Text = "";
                LblObj3Type.Text = "";
                LblObj3Rarete.Text = "";
                LblObj3Portée.Text = "";
                LblObj3Bonus.Text = "";
                LblObj3Effet.Text = "";
            }

            if (InventaireJoueur[2].Type == "Epee" || InventaireJoueur[2].Type == "Lance" || InventaireJoueur[2].Type == "Dague" || InventaireJoueur[2].Type == "Outil" || InventaireJoueur[2].Type == "Transcendance")
            {
                LblObj3Nom.ForeColor = Color.Red;
                LblObj3Type.ForeColor = Color.Red;
                LblObj3Rarete.ForeColor = Color.Red;
                LblObj3Portée.ForeColor = Color.Red;
                LblObj3Bonus.ForeColor = Color.Red;
                LblObj3Effet.ForeColor = Color.Red;
            }
            else
            {
                if (InventaireJoueur[2].Type == "Arc" || InventaireJoueur[2].Type == "Tome" || InventaireJoueur[2].Type == "Lancers" || InventaireJoueur[2].Type == "Arme a feu" || InventaireJoueur[2].Type == "Fronde")
                {
                    LblObj3Nom.ForeColor = Color.Blue;
                    LblObj3Type.ForeColor = Color.Blue;
                    LblObj3Rarete.ForeColor = Color.Blue;
                    LblObj3Portée.ForeColor = Color.Blue;
                    LblObj3Bonus.ForeColor = Color.Blue;
                    LblObj3Effet.ForeColor = Color.Blue;
                }
                else
                {
                    if (InventaireJoueur[2].Type == "Hache" || InventaireJoueur[2].Type == "Bouclier" || InventaireJoueur[2].Type == "Poutre" || InventaireJoueur[2].Type == "Pugilats" || InventaireJoueur[2].Type == "Bestipierre")
                    {
                        LblObj3Nom.ForeColor = Color.Green;
                        LblObj3Type.ForeColor = Color.Green;
                        LblObj3Rarete.ForeColor = Color.Green;
                        LblObj3Portée.ForeColor = Color.Green;
                        LblObj3Bonus.ForeColor = Color.Green;
                        LblObj3Effet.ForeColor = Color.Green;
                    }
                    else
                    {
                        LblObj3Nom.ForeColor = Color.Black;
                        LblObj3Type.ForeColor = Color.Black;
                        LblObj3Rarete.ForeColor = Color.Black;
                        LblObj3Portée.ForeColor = Color.Black;
                        LblObj3Bonus.ForeColor = Color.Black;
                        LblObj3Effet.ForeColor = Color.Black;
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (InventaireJoueur[3].Nom != "")
            {
                LblObj4Nom.Text = InventaireJoueur[3].Nom;
                LblObj4Type.Text = InventaireJoueur[3].Type;
                if (InventaireJoueur[3].Type == "Transcendance")
                    LblObj4Type.Text = "Transcend.";
                LblObj4Rarete.Text = InventaireJoueur[3].Rarete;
                LblObj4Portée.Text = InventaireJoueur[3].Portee;
                LblObj4Bonus.Text = InventaireJoueur[3].BonusType + " +" + InventaireJoueur[3].Bonus;

                if (InventaireJoueur[3].Type == "Soin" || InventaireJoueur[3].Type == "Degats" || InventaireJoueur[3].Type == "Buff" || InventaireJoueur[3].Type == "Debuff")
                {
                    LblObj4Nom.Text += " (" + InventaireJoueur[3].Quantité + ")";
                    LblObj4Bonus.Text = "          /";
                    LblObj4Effet.Text = InventaireJoueur[3].EffetDes1;
                }
                else
                {
                    if (InventaireJoueur[3].EffetType1 != "")
                    {
                        LblObj4Effet.Text = InventaireJoueur[3].EffetType1 + ": " + InventaireJoueur[3].EffetDes1;

                        if (InventaireJoueur[3].EffetType2 != "")
                        {
                            LblObj4Effet.Text += " ; " + InventaireJoueur[3].EffetType2 + ": " + InventaireJoueur[3].EffetDes2;

                            if (InventaireJoueur[3].EffetType3 != "")
                            {
                                LblObj4Effet.Text += " ; " + InventaireJoueur[3].EffetType3 + ": " + InventaireJoueur[3].EffetDes3;
                            }
                        }
                    }
                    else
                    {
                        LblObj4Effet.Text = "";
                    }
                }
            }
            else
            {
                LblObj4Nom.Text = "";
                LblObj4Type.Text = "";
                LblObj4Rarete.Text = "";
                LblObj4Portée.Text = "";
                LblObj4Bonus.Text = "";
                LblObj4Effet.Text = "";
            }

            if (InventaireJoueur[3].Type == "Epee" || InventaireJoueur[3].Type == "Lance" || InventaireJoueur[3].Type == "Dague" || InventaireJoueur[3].Type == "Outil" || InventaireJoueur[3].Type == "Transcendance")
            {
                LblObj4Nom.ForeColor = Color.Red;
                LblObj4Type.ForeColor = Color.Red;
                LblObj4Rarete.ForeColor = Color.Red;
                LblObj4Portée.ForeColor = Color.Red;
                LblObj4Bonus.ForeColor = Color.Red;
                LblObj4Effet.ForeColor = Color.Red;
            }
            else
            {
                if (InventaireJoueur[3].Type == "Arc" || InventaireJoueur[3].Type == "Tome" || InventaireJoueur[3].Type == "Lancers" || InventaireJoueur[3].Type == "Arme a feu" || InventaireJoueur[3].Type == "Fronde")
                {
                    LblObj4Nom.ForeColor = Color.Blue;
                    LblObj4Type.ForeColor = Color.Blue;
                    LblObj4Rarete.ForeColor = Color.Blue;
                    LblObj4Portée.ForeColor = Color.Blue;
                    LblObj4Bonus.ForeColor = Color.Blue;
                    LblObj4Effet.ForeColor = Color.Blue;
                }
                else
                {
                    if (InventaireJoueur[3].Type == "Hache" || InventaireJoueur[3].Type == "Bouclier" || InventaireJoueur[3].Type == "Poutre" || InventaireJoueur[3].Type == "Pugilats" || InventaireJoueur[3].Type == "Bestipierre")
                    {
                        LblObj4Nom.ForeColor = Color.Green;
                        LblObj4Type.ForeColor = Color.Green;
                        LblObj4Rarete.ForeColor = Color.Green;
                        LblObj4Portée.ForeColor = Color.Green;
                        LblObj4Bonus.ForeColor = Color.Green;
                        LblObj4Effet.ForeColor = Color.Green;
                    }
                    else
                    {
                        LblObj4Nom.ForeColor = Color.Black;
                        LblObj4Type.ForeColor = Color.Black;
                        LblObj4Rarete.ForeColor = Color.Black;
                        LblObj4Portée.ForeColor = Color.Black;
                        LblObj4Bonus.ForeColor = Color.Black;
                        LblObj4Effet.ForeColor = Color.Black;
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (InventaireJoueur[4].Nom != "")
            {
                LblObj5Nom.Text = InventaireJoueur[4].Nom;
                LblObj5Type.Text = InventaireJoueur[4].Type;
                if (InventaireJoueur[4].Type == "Transcendance")
                    LblObj5Type.Text = "Transcend.";
                LblObj5Rarete.Text = InventaireJoueur[4].Rarete;
                LblObj5Portée.Text = InventaireJoueur[4].Portee;
                LblObj5Bonus.Text = InventaireJoueur[4].BonusType + " +" + InventaireJoueur[4].Bonus;

                if (InventaireJoueur[4].Type == "Soin" || InventaireJoueur[4].Type == "Degats" || InventaireJoueur[4].Type == "Buff" || InventaireJoueur[4].Type == "Debuff")
                {
                    LblObj5Nom.Text += " (" + InventaireJoueur[4].Quantité + ")";
                    LblObj5Bonus.Text = "          /";
                    LblObj5Effet.Text = InventaireJoueur[4].EffetDes1;
                }
                else
                {
                    if (InventaireJoueur[4].EffetType1 != "")
                    {
                        LblObj5Effet.Text = InventaireJoueur[4].EffetType1 + ": " + InventaireJoueur[4].EffetDes1;

                        if (InventaireJoueur[4].EffetType2 != "")
                        {
                            LblObj5Effet.Text += " ; " + InventaireJoueur[4].EffetType2 + ": " + InventaireJoueur[4].EffetDes2;

                            if (InventaireJoueur[4].EffetType3 != "")
                            {
                                LblObj5Effet.Text += " ; " + InventaireJoueur[4].EffetType3 + ": " + InventaireJoueur[4].EffetDes3;
                            }
                        }
                    }
                    else
                    {
                        LblObj5Effet.Text = "";
                    }
                }
            }
            else
            {
                LblObj5Nom.Text = "";
                LblObj5Type.Text = "";
                LblObj5Rarete.Text = "";
                LblObj5Portée.Text = "";
                LblObj5Bonus.Text = "";
                LblObj5Effet.Text = "";
            }

            if (InventaireJoueur[4].Type == "Epee" || InventaireJoueur[4].Type == "Lance" || InventaireJoueur[4].Type == "Dague" || InventaireJoueur[4].Type == "Outil" || InventaireJoueur[4].Type == "Transcendance")
            {
                LblObj5Nom.ForeColor = Color.Red;
                LblObj5Type.ForeColor = Color.Red;
                LblObj5Rarete.ForeColor = Color.Red;
                LblObj5Portée.ForeColor = Color.Red;
                LblObj5Bonus.ForeColor = Color.Red;
                LblObj5Effet.ForeColor = Color.Red;
            }
            else
            {
                if (InventaireJoueur[4].Type == "Arc" || InventaireJoueur[4].Type == "Tome" || InventaireJoueur[4].Type == "Lancers" || InventaireJoueur[4].Type == "Arme a feu" || InventaireJoueur[4].Type == "Fronde")
                {
                    LblObj5Nom.ForeColor = Color.Blue;
                    LblObj5Type.ForeColor = Color.Blue;
                    LblObj5Rarete.ForeColor = Color.Blue;
                    LblObj5Portée.ForeColor = Color.Blue;
                    LblObj5Bonus.ForeColor = Color.Blue;
                    LblObj5Effet.ForeColor = Color.Blue;
                }
                else
                {
                    if (InventaireJoueur[4].Type == "Hache" || InventaireJoueur[4].Type == "Bouclier" || InventaireJoueur[4].Type == "Poutre" || InventaireJoueur[4].Type == "Pugilats" || InventaireJoueur[4].Type == "Bestipierre")
                    {
                        LblObj5Nom.ForeColor = Color.Green;
                        LblObj5Type.ForeColor = Color.Green;
                        LblObj5Rarete.ForeColor = Color.Green;
                        LblObj5Portée.ForeColor = Color.Green;
                        LblObj5Bonus.ForeColor = Color.Green;
                        LblObj5Effet.ForeColor = Color.Green;
                    }
                    else
                    {
                        LblObj5Nom.ForeColor = Color.Black;
                        LblObj5Type.ForeColor = Color.Black;
                        LblObj5Rarete.ForeColor = Color.Black;
                        LblObj5Portée.ForeColor = Color.Black;
                        LblObj5Bonus.ForeColor = Color.Black;
                        LblObj5Effet.ForeColor = Color.Black;
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (InventaireJoueur[5].Nom != "")
            {
                LblObj6Nom.Text = InventaireJoueur[5].Nom;
                LblObj6Type.Text = InventaireJoueur[5].Type;
                if (InventaireJoueur[5].Type == "Transcendance")
                    LblObj6Type.Text = "Transcend.";
                LblObj6Rarete.Text = InventaireJoueur[5].Rarete;
                LblObj6Portée.Text = InventaireJoueur[5].Portee;
                LblObj6Bonus.Text = InventaireJoueur[5].BonusType + " +" + InventaireJoueur[5].Bonus;

                if (InventaireJoueur[5].Type == "Soin" || InventaireJoueur[5].Type == "Degats" || InventaireJoueur[5].Type == "Buff" || InventaireJoueur[5].Type == "Debuff")
                {
                    LblObj6Nom.Text += " (" + InventaireJoueur[5].Quantité + ")";
                    LblObj6Bonus.Text = "          /";
                    LblObj6Effet.Text = InventaireJoueur[5].EffetDes1;
                }
                else
                {
                    if (InventaireJoueur[5].EffetType1 != "")
                    {
                        LblObj6Effet.Text = InventaireJoueur[5].EffetType1 + ": " + InventaireJoueur[5].EffetDes1;

                        if (InventaireJoueur[5].EffetType2 != "")
                        {
                            LblObj6Effet.Text += " ; " + InventaireJoueur[5].EffetType2 + ": " + InventaireJoueur[5].EffetDes2;

                            if (InventaireJoueur[5].EffetType3 != "")
                            {
                                LblObj6Effet.Text += " ; " + InventaireJoueur[5].EffetType3 + ": " + InventaireJoueur[5].EffetDes3;
                            }
                        }
                    }
                    else
                    {
                        LblObj6Effet.Text = "";
                    }
                }
            }
            else
            {
                LblObj6Nom.Text = "";
                LblObj6Type.Text = "";
                LblObj6Rarete.Text = "";
                LblObj6Portée.Text = "";
                LblObj6Bonus.Text = "";
                LblObj6Effet.Text = "";
            }

            if (InventaireJoueur[5].Type == "Epee" || InventaireJoueur[5].Type == "Lance" || InventaireJoueur[5].Type == "Dague" || InventaireJoueur[5].Type == "Outil" || InventaireJoueur[5].Type == "Transcendance")
            {
                LblObj6Nom.ForeColor = Color.Red;
                LblObj6Type.ForeColor = Color.Red;
                LblObj6Rarete.ForeColor = Color.Red;
                LblObj6Portée.ForeColor = Color.Red;
                LblObj6Bonus.ForeColor = Color.Red;
                LblObj6Effet.ForeColor = Color.Red;
            }
            else
            {
                if (InventaireJoueur[5].Type == "Arc" || InventaireJoueur[5].Type == "Tome" || InventaireJoueur[5].Type == "Lancers" || InventaireJoueur[5].Type == "Arme a feu" || InventaireJoueur[5].Type == "Fronde")
                {
                    LblObj6Nom.ForeColor = Color.Blue;
                    LblObj6Type.ForeColor = Color.Blue;
                    LblObj6Rarete.ForeColor = Color.Blue;
                    LblObj6Portée.ForeColor = Color.Blue;
                    LblObj6Bonus.ForeColor = Color.Blue;
                    LblObj6Effet.ForeColor = Color.Blue;
                }
                else
                {
                    if (InventaireJoueur[5].Type == "Hache" || InventaireJoueur[5].Type == "Bouclier" || InventaireJoueur[5].Type == "Poutre" || InventaireJoueur[5].Type == "Pugilats" || InventaireJoueur[5].Type == "Bestipierre")
                    {
                        LblObj6Nom.ForeColor = Color.Green;
                        LblObj6Type.ForeColor = Color.Green;
                        LblObj6Rarete.ForeColor = Color.Green;
                        LblObj6Portée.ForeColor = Color.Green;
                        LblObj6Bonus.ForeColor = Color.Green;
                        LblObj6Effet.ForeColor = Color.Green;
                    }
                    else
                    {
                        LblObj6Nom.ForeColor = Color.Black;
                        LblObj6Type.ForeColor = Color.Black;
                        LblObj6Rarete.ForeColor = Color.Black;
                        LblObj6Portée.ForeColor = Color.Black;
                        LblObj6Bonus.ForeColor = Color.Black;
                        LblObj6Effet.ForeColor = Color.Black;
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (InventaireJoueur[6].Nom != "")
            {
                LblObj7Nom.Text = InventaireJoueur[6].Nom;
                LblObj7Type.Text = InventaireJoueur[6].Type;
                if (InventaireJoueur[6].Type == "Transcendance")
                    LblObj7Type.Text = "Transcend.";
                LblObj7Rarete.Text = InventaireJoueur[6].Rarete;
                LblObj7Portée.Text = InventaireJoueur[6].Portee;
                LblObj7Bonus.Text = InventaireJoueur[6].BonusType + " +" + InventaireJoueur[6].Bonus;

                if (InventaireJoueur[6].Type == "Soin" || InventaireJoueur[6].Type == "Degats" || InventaireJoueur[6].Type == "Buff" || InventaireJoueur[6].Type == "Debuff")
                {
                    LblObj7Nom.Text += " (" + InventaireJoueur[6].Quantité + ")";
                    LblObj7Bonus.Text = "          /";
                    LblObj7Effet.Text = InventaireJoueur[6].EffetDes1;
                }
                else
                {
                    if (InventaireJoueur[6].EffetType1 != "")
                    {
                        LblObj7Effet.Text = InventaireJoueur[6].EffetType1 + ": " + InventaireJoueur[6].EffetDes1;

                        if (InventaireJoueur[6].EffetType2 != "")
                        {
                            LblObj7Effet.Text += " ; " + InventaireJoueur[6].EffetType2 + ": " + InventaireJoueur[6].EffetDes2;

                            if (InventaireJoueur[6].EffetType3 != "")
                            {
                                LblObj7Effet.Text += " ; " + InventaireJoueur[6].EffetType3 + ": " + InventaireJoueur[6].EffetDes3;
                            }
                        }
                    }
                    else
                    {
                        LblObj7Effet.Text = "";
                    }
                }
            }
            else
            {
                LblObj7Nom.Text = "";
                LblObj7Type.Text = "";
                LblObj7Rarete.Text = "";
                LblObj7Portée.Text = "";
                LblObj7Bonus.Text = "";
                LblObj7Effet.Text = "";
            }

            if (InventaireJoueur[6].Type == "Epee" || InventaireJoueur[6].Type == "Lance" || InventaireJoueur[6].Type == "Dague" || InventaireJoueur[6].Type == "Outil" || InventaireJoueur[6].Type == "Transcendance")
            {
                LblObj7Nom.ForeColor = Color.Red;
                LblObj7Type.ForeColor = Color.Red;
                LblObj7Rarete.ForeColor = Color.Red;
                LblObj7Portée.ForeColor = Color.Red;
                LblObj7Bonus.ForeColor = Color.Red;
                LblObj7Effet.ForeColor = Color.Red;
            }
            else
            {
                if (InventaireJoueur[6].Type == "Arc" || InventaireJoueur[6].Type == "Tome" || InventaireJoueur[6].Type == "Lancers" || InventaireJoueur[6].Type == "Arme a feu" || InventaireJoueur[6].Type == "Fronde")
                {
                    LblObj7Nom.ForeColor = Color.Blue;
                    LblObj7Type.ForeColor = Color.Blue;
                    LblObj7Rarete.ForeColor = Color.Blue;
                    LblObj7Portée.ForeColor = Color.Blue;
                    LblObj7Bonus.ForeColor = Color.Blue;
                    LblObj7Effet.ForeColor = Color.Blue;
                }
                else
                {
                    if (InventaireJoueur[6].Type == "Hache" || InventaireJoueur[6].Type == "Bouclier" || InventaireJoueur[6].Type == "Poutre" || InventaireJoueur[6].Type == "Pugilats" || InventaireJoueur[6].Type == "Bestipierre")
                    {
                        LblObj7Nom.ForeColor = Color.Green;
                        LblObj7Type.ForeColor = Color.Green;
                        LblObj7Rarete.ForeColor = Color.Green;
                        LblObj7Portée.ForeColor = Color.Green;
                        LblObj7Bonus.ForeColor = Color.Green;
                        LblObj7Effet.ForeColor = Color.Green;
                    }
                    else
                    {
                        LblObj7Nom.ForeColor = Color.Black;
                        LblObj7Type.ForeColor = Color.Black;
                        LblObj7Rarete.ForeColor = Color.Black;
                        LblObj7Portée.ForeColor = Color.Black;
                        LblObj7Bonus.ForeColor = Color.Black;
                        LblObj7Effet.ForeColor = Color.Black;
                    }
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (InventaireJoueur[7].Nom != "")
            {
                LblObj8Nom.Text = InventaireJoueur[7].Nom;
                LblObj8Type.Text = InventaireJoueur[7].Type;
                if (InventaireJoueur[7].Type == "Transcendance")
                    LblObj8Type.Text = "Transcend.";
                LblObj8Rarete.Text = InventaireJoueur[7].Rarete;
                LblObj8Portée.Text = InventaireJoueur[7].Portee;
                LblObj8Bonus.Text = InventaireJoueur[7].BonusType + " +" + InventaireJoueur[7].Bonus;

                if (InventaireJoueur[7].Type == "Soin" || InventaireJoueur[7].Type == "Degats" || InventaireJoueur[7].Type == "Buff" || InventaireJoueur[7].Type == "Debuff")
                {
                    LblObj8Nom.Text += " (" + InventaireJoueur[7].Quantité + ")";
                    LblObj8Bonus.Text = "          /";
                    LblObj8Effet.Text = InventaireJoueur[7].EffetDes1;
                }
                else
                {
                    if (InventaireJoueur[7].EffetType1 != "")
                    {
                        LblObj8Effet.Text = InventaireJoueur[7].EffetType1 + ": " + InventaireJoueur[7].EffetDes1;

                        if (InventaireJoueur[7].EffetType2 != "")
                        {
                            LblObj8Effet.Text += " ; " + InventaireJoueur[7].EffetType2 + ": " + InventaireJoueur[7].EffetDes2;

                            if (InventaireJoueur[7].EffetType3 != "")
                            {
                                LblObj8Effet.Text += " ; " + InventaireJoueur[7].EffetType3 + ": " + InventaireJoueur[7].EffetDes3;
                            }
                        }
                    }
                    else
                    {
                        LblObj8Effet.Text = "";
                    }
                }
            }
            else
            {
                LblObj8Nom.Text = "";
                LblObj8Type.Text = "";
                LblObj8Rarete.Text = "";
                LblObj8Portée.Text = "";
                LblObj8Bonus.Text = "";
                LblObj8Effet.Text = "";
            }

            if (InventaireJoueur[7].Type == "Epee" || InventaireJoueur[7].Type == "Lance" || InventaireJoueur[7].Type == "Dague" || InventaireJoueur[7].Type == "Outil" || InventaireJoueur[7].Type == "Transcendance")
            {
                LblObj8Nom.ForeColor = Color.Red;
                LblObj8Type.ForeColor = Color.Red;
                LblObj8Rarete.ForeColor = Color.Red;
                LblObj8Portée.ForeColor = Color.Red;
                LblObj8Bonus.ForeColor = Color.Red;
                LblObj8Effet.ForeColor = Color.Red;
            }
            else
            {
                if (InventaireJoueur[7].Type == "Arc" || InventaireJoueur[7].Type == "Tome" || InventaireJoueur[7].Type == "Lancers" || InventaireJoueur[7].Type == "Arme a feu" || InventaireJoueur[7].Type == "Fronde")
                {
                    LblObj8Nom.ForeColor = Color.Blue;
                    LblObj8Type.ForeColor = Color.Blue;
                    LblObj8Rarete.ForeColor = Color.Blue;
                    LblObj8Portée.ForeColor = Color.Blue;
                    LblObj8Bonus.ForeColor = Color.Blue;
                    LblObj8Effet.ForeColor = Color.Blue;
                }
                else
                {
                    if (InventaireJoueur[7].Type == "Hache" || InventaireJoueur[7].Type == "Bouclier" || InventaireJoueur[7].Type == "Poutre" || InventaireJoueur[7].Type == "Pugilats" || InventaireJoueur[7].Type == "Bestipierre")
                    {
                        LblObj8Nom.ForeColor = Color.Green;
                        LblObj8Type.ForeColor = Color.Green;
                        LblObj8Rarete.ForeColor = Color.Green;
                        LblObj8Portée.ForeColor = Color.Green;
                        LblObj8Bonus.ForeColor = Color.Green;
                        LblObj8Effet.ForeColor = Color.Green;
                    }
                    else
                    {
                        LblObj8Nom.ForeColor = Color.Black;
                        LblObj8Type.ForeColor = Color.Black;
                        LblObj8Rarete.ForeColor = Color.Black;
                        LblObj8Portée.ForeColor = Color.Black;
                        LblObj8Bonus.ForeColor = Color.Black;
                        LblObj8Effet.ForeColor = Color.Black;
                    }
                }
            }
        }

        //Fonction actualisation TxtBox Chx Activité Chx Effectué

        public void actualisationTxtBoxChxActivitéChxEffectué()
        {
            TxtBoxChxActivitéChoixEffectué.Text = "Activité sélectionnée : " + chx_activite;
        }

        //Fonction changement Date

        public void chgtDate()
        {
            if (MomentJournée == "Matin")
            {
                MomentJournée = "Journée";
            }
            else
            {
                if (MomentJournée == "Journée")
                {
                    MomentJournée = "Soir";
                }
                else
                {
                    MomentJournée = "Matin";
                    Date++;
                }
            }
        }

        //Fonction calcul Stats Tot

        public void calculStatsTot()
        {
            ClassePerso.PVEquip = 0;
            ClassePerso.PreEquip = 0;
            ClassePerso.EsqEquip = 0;
            ClassePerso.CritEquip = 0;
            ClassePerso.VitEquip = 0;
            ClassePerso.ConsEquip = 0;
            ClassePerso.IntEquip = 0;
            ClassePerso.ChaEquip = 0;
            ClassePerso.SauEquip = 0;
            ClassePerso.InsEquip = 0;

            if (InventaireJoueur[0].Type == ClassePerso.Equipable1 || InventaireJoueur[0].Type == ClassePerso.Equipable2 || InventaireJoueur[0].Type == ClassePerso.Equipable3 || InventaireJoueur[0].Type == ClassePerso.Equipable4 || InventaireJoueur[0].Type == ClassePerso.Equipable5 || InventaireJoueur[0].Type == ClassePerso.Equipable6)
            {
                switch (InventaireJoueur[0].BonusType)
                {
                    case "Pre":
                        ClassePerso.PreEquip += InventaireJoueur[0].Bonus;
                        break;
                    case "Esq":
                        ClassePerso.EsqEquip += InventaireJoueur[0].Bonus;
                        break;
                    case "Crit":
                        ClassePerso.CritEquip += InventaireJoueur[0].Bonus;
                        break;
                    case "Vit":
                        ClassePerso.VitEquip += InventaireJoueur[0].Bonus;
                        break;
                    case "Cons":
                        ClassePerso.ConsEquip += InventaireJoueur[0].Bonus;
                        break;
                    case "Int":
                        ClassePerso.IntEquip += InventaireJoueur[0].Bonus;
                        break;
                    case "Cha":
                        ClassePerso.ChaEquip += InventaireJoueur[0].Bonus;
                        break;
                    case "Sau":
                        ClassePerso.SauEquip += InventaireJoueur[0].Bonus;
                        break;
                    default:
                        ClassePerso.InsEquip += InventaireJoueur[0].Bonus;
                        break;
                }
            }

            if (InventaireJoueur[1].Type == ClassePerso.Equipable1 || InventaireJoueur[1].Type == ClassePerso.Equipable2 || InventaireJoueur[1].Type == ClassePerso.Equipable3 || InventaireJoueur[1].Type == ClassePerso.Equipable4 || InventaireJoueur[1].Type == ClassePerso.Equipable5 || InventaireJoueur[1].Type == ClassePerso.Equipable6)
            {
                switch (InventaireJoueur[1].BonusType)
                {
                    case "Pre":
                        ClassePerso.PreEquip += InventaireJoueur[1].Bonus;
                        break;
                    case "Esq":
                        ClassePerso.EsqEquip += InventaireJoueur[1].Bonus;
                        break;
                    case "Crit":
                        ClassePerso.CritEquip += InventaireJoueur[1].Bonus;
                        break;
                    case "Vit":
                        ClassePerso.VitEquip += InventaireJoueur[1].Bonus;
                        break;
                    case "Cons":
                        ClassePerso.ConsEquip += InventaireJoueur[1].Bonus;
                        break;
                    case "Int":
                        ClassePerso.IntEquip += InventaireJoueur[1].Bonus;
                        break;
                    case "Cha":
                        ClassePerso.ChaEquip += InventaireJoueur[1].Bonus;
                        break;
                    case "Sau":
                        ClassePerso.SauEquip += InventaireJoueur[1].Bonus;
                        break;
                    default:
                        ClassePerso.InsEquip += InventaireJoueur[1].Bonus;
                        break;
                }
            }

            if (InventaireJoueur[2].Type == ClassePerso.Equipable1 || InventaireJoueur[2].Type == ClassePerso.Equipable2 || InventaireJoueur[2].Type == ClassePerso.Equipable3 || InventaireJoueur[2].Type == ClassePerso.Equipable4 || InventaireJoueur[2].Type == ClassePerso.Equipable5 || InventaireJoueur[2].Type == ClassePerso.Equipable6)
            {
                switch (InventaireJoueur[2].BonusType)
                {
                    case "Pre":
                        ClassePerso.PreEquip += InventaireJoueur[2].Bonus;
                        break;
                    case "Esq":
                        ClassePerso.EsqEquip += InventaireJoueur[2].Bonus;
                        break;
                    case "Crit":
                        ClassePerso.CritEquip += InventaireJoueur[2].Bonus;
                        break;
                    case "Vit":
                        ClassePerso.VitEquip += InventaireJoueur[2].Bonus;
                        break;
                    case "Cons":
                        ClassePerso.ConsEquip += InventaireJoueur[2].Bonus;
                        break;
                    case "Int":
                        ClassePerso.IntEquip += InventaireJoueur[2].Bonus;
                        break;
                    case "Cha":
                        ClassePerso.ChaEquip += InventaireJoueur[2].Bonus;
                        break;
                    case "Sau":
                        ClassePerso.SauEquip += InventaireJoueur[2].Bonus;
                        break;
                    default:
                        ClassePerso.InsEquip += InventaireJoueur[2].Bonus;
                        break;
                }
            }

            if (InventaireJoueur[3].Type == ClassePerso.Equipable1 || InventaireJoueur[3].Type == ClassePerso.Equipable2 || InventaireJoueur[3].Type == ClassePerso.Equipable3 || InventaireJoueur[3].Type == ClassePerso.Equipable4 || InventaireJoueur[3].Type == ClassePerso.Equipable5 || InventaireJoueur[3].Type == ClassePerso.Equipable6)
            {
                switch (InventaireJoueur[3].BonusType)
                {
                    case "Pre":
                        ClassePerso.PreEquip += InventaireJoueur[3].Bonus;
                        break;
                    case "Esq":
                        ClassePerso.EsqEquip += InventaireJoueur[3].Bonus;
                        break;
                    case "Crit":
                        ClassePerso.CritEquip += InventaireJoueur[3].Bonus;
                        break;
                    case "Vit":
                        ClassePerso.VitEquip += InventaireJoueur[3].Bonus;
                        break;
                    case "Cons":
                        ClassePerso.ConsEquip += InventaireJoueur[3].Bonus;
                        break;
                    case "Int":
                        ClassePerso.IntEquip += InventaireJoueur[3].Bonus;
                        break;
                    case "Cha":
                        ClassePerso.ChaEquip += InventaireJoueur[3].Bonus;
                        break;
                    case "Sau":
                        ClassePerso.SauEquip += InventaireJoueur[3].Bonus;
                        break;
                    default:
                        ClassePerso.InsEquip += InventaireJoueur[3].Bonus;
                        break;
                }
            }

            if (InventaireJoueur[4].Type == ClassePerso.Equipable1 || InventaireJoueur[4].Type == ClassePerso.Equipable2 || InventaireJoueur[4].Type == ClassePerso.Equipable3 || InventaireJoueur[4].Type == ClassePerso.Equipable4 || InventaireJoueur[4].Type == ClassePerso.Equipable5 || InventaireJoueur[4].Type == ClassePerso.Equipable6)
            {
                switch (InventaireJoueur[4].BonusType)
                {
                    case "Pre":
                        ClassePerso.PreEquip += InventaireJoueur[4].Bonus;
                        break;
                    case "Esq":
                        ClassePerso.EsqEquip += InventaireJoueur[4].Bonus;
                        break;
                    case "Crit":
                        ClassePerso.CritEquip += InventaireJoueur[4].Bonus;
                        break;
                    case "Vit":
                        ClassePerso.VitEquip += InventaireJoueur[4].Bonus;
                        break;
                    case "Cons":
                        ClassePerso.ConsEquip += InventaireJoueur[4].Bonus;
                        break;
                    case "Int":
                        ClassePerso.IntEquip += InventaireJoueur[4].Bonus;
                        break;
                    case "Cha":
                        ClassePerso.ChaEquip += InventaireJoueur[4].Bonus;
                        break;
                    case "Sau":
                        ClassePerso.SauEquip += InventaireJoueur[4].Bonus;
                        break;
                    default:
                        ClassePerso.InsEquip += InventaireJoueur[4].Bonus;
                        break;
                }
            }

            if (InventaireJoueur[5].Type == ClassePerso.Equipable1 || InventaireJoueur[5].Type == ClassePerso.Equipable2 || InventaireJoueur[5].Type == ClassePerso.Equipable3 || InventaireJoueur[5].Type == ClassePerso.Equipable4 || InventaireJoueur[5].Type == ClassePerso.Equipable5 || InventaireJoueur[5].Type == ClassePerso.Equipable6)
            {
                switch (InventaireJoueur[5].BonusType)
                {
                    case "Pre":
                        ClassePerso.PreEquip += InventaireJoueur[5].Bonus;
                        break;
                    case "Esq":
                        ClassePerso.EsqEquip += InventaireJoueur[5].Bonus;
                        break;
                    case "Crit":
                        ClassePerso.CritEquip += InventaireJoueur[5].Bonus;
                        break;
                    case "Vit":
                        ClassePerso.VitEquip += InventaireJoueur[5].Bonus;
                        break;
                    case "Cons":
                        ClassePerso.ConsEquip += InventaireJoueur[5].Bonus;
                        break;
                    case "Int":
                        ClassePerso.IntEquip += InventaireJoueur[5].Bonus;
                        break;
                    case "Cha":
                        ClassePerso.ChaEquip += InventaireJoueur[5].Bonus;
                        break;
                    case "Sau":
                        ClassePerso.SauEquip += InventaireJoueur[5].Bonus;
                        break;
                    default:
                        ClassePerso.InsEquip += InventaireJoueur[5].Bonus;
                        break;
                }
            }

            if (InventaireJoueur[6].Type == ClassePerso.Equipable1 || InventaireJoueur[6].Type == ClassePerso.Equipable2 || InventaireJoueur[6].Type == ClassePerso.Equipable3 || InventaireJoueur[6].Type == ClassePerso.Equipable4 || InventaireJoueur[6].Type == ClassePerso.Equipable5 || InventaireJoueur[6].Type == ClassePerso.Equipable6)
            {
                switch (InventaireJoueur[6].BonusType)
                {
                    case "Pre":
                        ClassePerso.PreEquip += InventaireJoueur[6].Bonus;
                        break;
                    case "Esq":
                        ClassePerso.EsqEquip += InventaireJoueur[6].Bonus;
                        break;
                    case "Crit":
                        ClassePerso.CritEquip += InventaireJoueur[6].Bonus;
                        break;
                    case "Vit":
                        ClassePerso.VitEquip += InventaireJoueur[6].Bonus;
                        break;
                    case "Cons":
                        ClassePerso.ConsEquip += InventaireJoueur[6].Bonus;
                        break;
                    case "Int":
                        ClassePerso.IntEquip += InventaireJoueur[6].Bonus;
                        break;
                    case "Cha":
                        ClassePerso.ChaEquip += InventaireJoueur[6].Bonus;
                        break;
                    case "Sau":
                        ClassePerso.SauEquip += InventaireJoueur[6].Bonus;
                        break;
                    default:
                        ClassePerso.InsEquip += InventaireJoueur[6].Bonus;
                        break;
                }
            }

            if (InventaireJoueur[7].Type == ClassePerso.Equipable1 || InventaireJoueur[7].Type == ClassePerso.Equipable2 || InventaireJoueur[7].Type == ClassePerso.Equipable3 || InventaireJoueur[7].Type == ClassePerso.Equipable4 || InventaireJoueur[7].Type == ClassePerso.Equipable5 || InventaireJoueur[7].Type == ClassePerso.Equipable6)
            {
                switch (InventaireJoueur[7].BonusType)
                {
                    case "Pre":
                        ClassePerso.PreEquip += InventaireJoueur[7].Bonus;
                        break;
                    case "Esq":
                        ClassePerso.EsqEquip += InventaireJoueur[7].Bonus;
                        break;
                    case "Crit":
                        ClassePerso.CritEquip += InventaireJoueur[7].Bonus;
                        break;
                    case "Vit":
                        ClassePerso.VitEquip += InventaireJoueur[7].Bonus;
                        break;
                    case "Cons":
                        ClassePerso.ConsEquip += InventaireJoueur[7].Bonus;
                        break;
                    case "Int":
                        ClassePerso.IntEquip += InventaireJoueur[7].Bonus;
                        break;
                    case "Cha":
                        ClassePerso.ChaEquip += InventaireJoueur[7].Bonus;
                        break;
                    case "Sau":
                        ClassePerso.SauEquip += InventaireJoueur[7].Bonus;
                        break;
                    default:
                        ClassePerso.InsEquip += InventaireJoueur[7].Bonus;
                        break;
                }
            }

            if (CompJoueur[0].Type == "Passive")
            {
                ClassePerso.PVEquip += CompJoueur[0].BonusPV;
                ClassePerso.PreEquip += CompJoueur[0].BonusPre;
                ClassePerso.EsqEquip += CompJoueur[0].BonusEsq;
                ClassePerso.CritEquip += CompJoueur[0].BonusCrit;
                ClassePerso.VitEquip += CompJoueur[0].BonusVit;
                ClassePerso.ConsEquip += CompJoueur[0].BonusCons;
                ClassePerso.IntEquip += CompJoueur[0].BonusInt;
                ClassePerso.ChaEquip += CompJoueur[0].BonusCha;
                ClassePerso.SauEquip += CompJoueur[0].BonusSau;
                ClassePerso.InsEquip += CompJoueur[0].BonusIns;
            }

            if (CompJoueur[1].Type == "Passive")
            {
                ClassePerso.PVEquip += CompJoueur[1].BonusPV;
                ClassePerso.PreEquip += CompJoueur[1].BonusPre;
                ClassePerso.EsqEquip += CompJoueur[1].BonusEsq;
                ClassePerso.CritEquip += CompJoueur[1].BonusCrit;
                ClassePerso.VitEquip += CompJoueur[1].BonusVit;
                ClassePerso.ConsEquip += CompJoueur[1].BonusCons;
                ClassePerso.IntEquip += CompJoueur[1].BonusInt;
                ClassePerso.ChaEquip += CompJoueur[1].BonusCha;
                ClassePerso.SauEquip += CompJoueur[1].BonusSau;
                ClassePerso.InsEquip += CompJoueur[1].BonusIns;
            }

            if (CompJoueur[2].Type == "Passive")
            {
                ClassePerso.PVEquip += CompJoueur[2].BonusPV;
                ClassePerso.PreEquip += CompJoueur[2].BonusPre;
                ClassePerso.EsqEquip += CompJoueur[2].BonusEsq;
                ClassePerso.CritEquip += CompJoueur[2].BonusCrit;
                ClassePerso.VitEquip += CompJoueur[2].BonusVit;
                ClassePerso.ConsEquip += CompJoueur[2].BonusCons;
                ClassePerso.IntEquip += CompJoueur[2].BonusInt;
                ClassePerso.ChaEquip += CompJoueur[2].BonusCha;
                ClassePerso.SauEquip += CompJoueur[2].BonusSau;
                ClassePerso.InsEquip += CompJoueur[2].BonusIns;
            }

            if (CompJoueur[3].Type == "Passive")
            {
                ClassePerso.PVEquip += CompJoueur[3].BonusPV;
                ClassePerso.PreEquip += CompJoueur[3].BonusPre;
                ClassePerso.EsqEquip += CompJoueur[3].BonusEsq;
                ClassePerso.CritEquip += CompJoueur[3].BonusCrit;
                ClassePerso.VitEquip += CompJoueur[3].BonusVit;
                ClassePerso.ConsEquip += CompJoueur[3].BonusCons;
                ClassePerso.IntEquip += CompJoueur[3].BonusInt;
                ClassePerso.ChaEquip += CompJoueur[3].BonusCha;
                ClassePerso.SauEquip += CompJoueur[3].BonusSau;
                ClassePerso.InsEquip += CompJoueur[3].BonusIns;
            }

            if (CompJoueur[4].Type == "Passive")
            {
                ClassePerso.PVEquip += CompJoueur[4].BonusPV;
                ClassePerso.PreEquip += CompJoueur[4].BonusPre;
                ClassePerso.EsqEquip += CompJoueur[4].BonusEsq;
                ClassePerso.CritEquip += CompJoueur[4].BonusCrit;
                ClassePerso.VitEquip += CompJoueur[4].BonusVit;
                ClassePerso.ConsEquip += CompJoueur[4].BonusCons;
                ClassePerso.IntEquip += CompJoueur[4].BonusInt;
                ClassePerso.ChaEquip += CompJoueur[4].BonusCha;
                ClassePerso.SauEquip += CompJoueur[4].BonusSau;
                ClassePerso.InsEquip += CompJoueur[4].BonusIns;
            }

            if (CompJoueur[5].Type == "Passive")
            {
                ClassePerso.PVEquip += CompJoueur[5].BonusPV;
                ClassePerso.PreEquip += CompJoueur[5].BonusPre;
                ClassePerso.EsqEquip += CompJoueur[5].BonusEsq;
                ClassePerso.CritEquip += CompJoueur[5].BonusCrit;
                ClassePerso.VitEquip += CompJoueur[5].BonusVit;
                ClassePerso.ConsEquip += CompJoueur[5].BonusCons;
                ClassePerso.IntEquip += CompJoueur[5].BonusInt;
                ClassePerso.ChaEquip += CompJoueur[5].BonusCha;
                ClassePerso.SauEquip += CompJoueur[5].BonusSau;
                ClassePerso.InsEquip += CompJoueur[5].BonusIns;
            }

            if (CompJoueur[6].Type == "Passive")
            {
                ClassePerso.PVEquip += CompJoueur[6].BonusPV;
                ClassePerso.PreEquip += CompJoueur[6].BonusPre;
                ClassePerso.EsqEquip += CompJoueur[6].BonusEsq;
                ClassePerso.CritEquip += CompJoueur[6].BonusCrit;
                ClassePerso.VitEquip += CompJoueur[6].BonusVit;
                ClassePerso.ConsEquip += CompJoueur[6].BonusCons;
                ClassePerso.IntEquip += CompJoueur[6].BonusInt;
                ClassePerso.ChaEquip += CompJoueur[6].BonusCha;
                ClassePerso.SauEquip += CompJoueur[6].BonusSau;
                ClassePerso.InsEquip += CompJoueur[6].BonusIns;
            }

            if (CompJoueur[7].Type == "Passive")
            {
                ClassePerso.PVEquip += CompJoueur[7].BonusPV;
                ClassePerso.PreEquip += CompJoueur[7].BonusPre;
                ClassePerso.EsqEquip += CompJoueur[7].BonusEsq;
                ClassePerso.CritEquip += CompJoueur[7].BonusCrit;
                ClassePerso.VitEquip += CompJoueur[7].BonusVit;
                ClassePerso.ConsEquip += CompJoueur[7].BonusCons;
                ClassePerso.IntEquip += CompJoueur[7].BonusInt;
                ClassePerso.ChaEquip += CompJoueur[7].BonusCha;
                ClassePerso.SauEquip += CompJoueur[7].BonusSau;
                ClassePerso.InsEquip += CompJoueur[7].BonusIns;
            }

            if (Talent == "Soigneux")
            {
                ClassePerso.PVEquip = (int)(ClassePerso.PVEquip * 1.1);
                ClassePerso.PreEquip = (int)(ClassePerso.PreEquip * 1.1);
                ClassePerso.EsqEquip = (int)(ClassePerso.EsqEquip * 1.1);
                ClassePerso.CritEquip = (int)(ClassePerso.CritEquip * 1.1);
                ClassePerso.VitEquip = (int)(ClassePerso.VitEquip * 1.1);
                ClassePerso.ConsEquip = (int)(ClassePerso.ConsEquip * 1.1);
                ClassePerso.IntEquip = (int)(ClassePerso.IntEquip * 1.1);
                ClassePerso.ChaEquip = (int)(ClassePerso.ChaEquip * 1.1);
                ClassePerso.SauEquip = (int)(ClassePerso.SauEquip * 1.1);
                ClassePerso.InsEquip = (int)(ClassePerso.InsEquip * 1.1);
            }

            ClassePerso.PVTotMax = ClassePerso.PVBase + ClassePerso.PVEquip;

            if (Phase == "d'enquete")
                ClassePerso.PVTotAct = ClassePerso.PVTotMax;

            ClassePerso.MvtTot = ClassePerso.MvtBase;
            if (ClassePerso.MvtBuff == 1)
            {
                if (ClassePerso.MvtBase == "S")
                {
                    ClassePerso.MvtTot = "M";
                }
                else
                {
                    ClassePerso.MvtTot = "L";
                }
            }

            ClassePerso.SocialTot = ClassePerso.SocialBase;
            ClassePerso.PreTot = ClassePerso.PreBase + ClassePerso.PreBuff + ClassePerso.PreEquip;
            ClassePerso.EsqTot = ClassePerso.EsqBase + ClassePerso.EsqBuff + ClassePerso.EsqEquip;
            ClassePerso.CritTot = ClassePerso.CritBase + ClassePerso.CritBuff + ClassePerso.CritEquip;

            ClassePerso.VitTot = ClassePerso.VitBase + ClassePerso.VitBuff + ClassePerso.VitEquip;
            ClassePerso.ConsTot = ClassePerso.ConsBase + ClassePerso.ConsBuff + ClassePerso.ConsEquip;
            ClassePerso.IntTot = ClassePerso.IntBase + ClassePerso.IntBuff + ClassePerso.IntEquip;
            ClassePerso.ChaTot = ClassePerso.ChaBase + ClassePerso.ChaBuff + ClassePerso.ChaEquip;
            ClassePerso.SauTot = ClassePerso.SauBase + ClassePerso.SauBuff + ClassePerso.SauEquip;
            ClassePerso.InsTot = ClassePerso.InsBase + ClassePerso.InsBuff + ClassePerso.InsEquip;
        }

        //Fonction sélection Rareté Chapitre

        public void selecRareteChapitre()
        {
            switch (Chapitre)
            {
                case 1:
                    BtnMarchéAchatChxRareté3.Visible = false;
                    BtnMarchéAchatChxRareté4.Visible = false;
                    BtnMarchéAchatChxRareté5.Visible = false;
                    BtnMarchéAchatChxRaretéEX.Visible = false;
                    break;
                case 2:
                    BtnMarchéAchatChxRareté3.Visible = true;
                    BtnMarchéAchatChxRareté4.Visible = false;
                    BtnMarchéAchatChxRareté5.Visible = false;
                    BtnMarchéAchatChxRaretéEX.Visible = false;
                    break;
                case 3:
                    BtnMarchéAchatChxRareté3.Visible = true;
                    BtnMarchéAchatChxRareté4.Visible = true;
                    BtnMarchéAchatChxRareté5.Visible = false;
                    BtnMarchéAchatChxRaretéEX.Visible = false;
                    break;
                case 4:
                    BtnMarchéAchatChxRareté3.Visible = true;
                    BtnMarchéAchatChxRareté4.Visible = true;
                    BtnMarchéAchatChxRareté5.Visible = true;
                    BtnMarchéAchatChxRaretéEX.Visible = false;
                    break;
                default:
                    BtnMarchéAchatChxRareté3.Visible = true;
                    BtnMarchéAchatChxRareté4.Visible = true;
                    BtnMarchéAchatChxRareté5.Visible = true;
                    BtnMarchéAchatChxRaretéEX.Visible = true;
                    break;
            }
        }

        //Fonction achat Objet Marché

        public void achatObjetMarché(ref int fonds, ref ObjetInv[] inventairejoueur, ref ObjetInv objetàacheter)
        {
            if(objetàacheter.Type != "Comp")
            {
                if (objetàacheter.Nom != "Decoction de rapidite" && objetàacheter.Nom != "Viagra" && objetàacheter.Nom != "Sushi" && objetàacheter.Nom != "Trefle" && objetàacheter.Nom != "Wyverne ivre" && objetàacheter.Nom != "Traumatisme" && objetàacheter.Nom != "Carotte doree" && objetàacheter.Nom != "Adrenaline" && objetàacheter.Nom != "49-3" && objetàacheter.Nom != "Sirop a la menthe" && objetàacheter.Nom != "Parfum" && objetàacheter.Nom != "Bottes des sept lieues")
                {
                    int i = 0;

                    while (inventairejoueur[i].EmplOccupe != false)
                    {
                        i++;
                    }

                    inventairejoueur[i] = objetàacheter;
                    inventairejoueur[i].Quantité++;
                }
                else
                {
                    switch(objetàacheter.Nom)
                    {
                        case "Decoction de rapidite":
                            ClassePerso.VitBase++;
                            break;
                        case "Viagra":
                            ClassePerso.ConsBase++;
                            break;
                        case "Sushi":
                            ClassePerso.IntBase++;
                            break;
                        case "Trefle":
                            ClassePerso.ChaBase++;
                            break;
                        case "Wyverne ivre":
                            ClassePerso.SauBase++;
                            break;
                        case "Traumatisme":
                            ClassePerso.InsBase++;
                            break;
                        case "Carotte doree":
                            ClassePerso.PreBase++;
                            break;
                        case "Adrenaline":
                            ClassePerso.EsqBase++;
                            break;
                        case "49-3":
                            ClassePerso.CritBase++;
                            break;
                        case "Sirop a la menthe":
                            ClassePerso.PVBase++;
                            break;
                        case "Parfum":
                            ClassePerso.SocialBase++;
                            break;
                        default:
                            break;
                    }

                    if(objetàacheter.Nom == "Bottes des sept lieues" && ClassePerso.MvtBase != "L")
                    {
                        if(ClassePerso.MvtBase == "S")
                        {
                            ClassePerso.MvtBase = "M";
                        }
                        else
                        {
                            ClassePerso.MvtBase = "L";
                        }
                    }
                    else
                    {
                        if(objetàacheter.Nom == "Bottes des sept lieues" && ClassePerso.MvtBase == "L")
                        {
                            fonds += objetàacheter.Prix;
                        }
                    }
                }
            }
            else
            {
                comp_marche = true;

                switch(objetàacheter.Nom)
                {
                    case "Custom A":
                        fctCompCustom("A");
                        break;
                    case "Custom B":
                        fctCompCustom("B");
                        break;
                    case "Inversion A":
                        //
                        break;
                    case "Inversion B":
                        //
                        break;
                    case "Inversion C":
                        //
                        break;
                    case "Defaitiste":
                        //
                        break;
                    case "Inversion D":
                        //
                        break;
                    case "Inversion E":
                        //
                        break;
                    case "Inversion F":
                        //
                        break;
                    case "Inversion G":
                        //
                        break;
                    case "Inversion H":
                        //
                        break;
                    case "Inversion I":
                        //
                        break;
                    case "Ecu":
                        //
                        break;
                    case "Glaive":
                        //
                        break;
                    case "Custom C":
                        fctCompCustom("C");
                        break;
                    case "Inversion X":
                        //
                        break;
                    case "Custom Z":
                        fctCompCustom("Z");
                        break;
                    case "Inversion Z":
                        //
                        break;
                    default:
                        //
                        break;
                }
            }

            fonds -= objetàacheter.Prix;
            actualisationLblFondsMarché();
        }

        public void achatObjetMarché(ref int fonds, ref ObjetInv inventairejoueur, ref ObjetInv objetàacheter)
        {
            fonds -= objetàacheter.Prix;
            inventairejoueur.Quantité++;
            actualisationLblFondsMarché();
        }

        //Fonction ajout Objet Alchimie

        public void ajoutObjetAlchimie(ref int qte, ref ObjetInv[] inventairejoueur, ref ObjetInv objetàacheter)
        {
            int i = 0;

            if (qte > 3)
            {
                while (inventairejoueur[i].EmplOccupe != false)
                {
                    i++;
                }

                inventairejoueur[i] = objetàacheter;
                inventairejoueur[i].Quantité = 3;

                i = 0;
                qte -= 3;

                while (inventairejoueur[i].EmplOccupe != false)
                {
                    i++;
                }

                inventairejoueur[i] = objetàacheter;
                inventairejoueur[i].Quantité = qte;
            }
            else
            {
                while (inventairejoueur[i].EmplOccupe != false)
                {
                    i++;
                }

                inventairejoueur[i] = objetàacheter;
                inventairejoueur[i].Quantité = qte;
            }
        }

        //Fonction vente Objet Marché

        public void venteObjetMarché(ref int fonds, ref ObjetInv inventairejoueur)
        {
            fonds += inventairejoueur.Prix / 2;
            inventairejoueur.Quantité--;

            if (inventairejoueur.Quantité == 0)
            {
                inventairejoueur.Type = "";
                inventairejoueur.Nom = "";
                inventairejoueur.Rarete = "";
                inventairejoueur.Portee = "";
                inventairejoueur.BonusType = "";
                inventairejoueur.Bonus = 0;
                inventairejoueur.EffetType1 = "";
                inventairejoueur.EffetDes1 = "";
                inventairejoueur.EffetType2 = "";
                inventairejoueur.EffetDes2 = "";
                inventairejoueur.EffetType3 = "";
                inventairejoueur.EffetDes3 = "";
                inventairejoueur.Prix = 0;
                inventairejoueur.EmplOccupe = false;
            }

            actualisationLblFondsMarché();
        }

        //Fonction actualisation Panel Remplacement Comp

        public void actualisationPnlRemplacementComp()
        {
            BtnRemplacementComp1.Text = CompJoueur[0].Nom;
            BtnRemplacementComp2.Text = CompJoueur[1].Nom;
            BtnRemplacementComp3.Text = CompJoueur[2].Nom;
            BtnRemplacementComp4.Text = CompJoueur[3].Nom;
            BtnRemplacementComp5.Text = CompJoueur[4].Nom;
            BtnRemplacementComp6.Text = CompJoueur[5].Nom;
            BtnRemplacementComp7.Text = CompJoueur[6].Nom;
            BtnRemplacementComp8.Text = CompJoueur[7].Nom;
            BtnRemplacementCompSup.Text = new_comp.Nom;

            LblRemplacementCompCD1.Text = CompJoueur[0].CD.ToString();
            LblRemplacementCompCD2.Text = CompJoueur[1].CD.ToString();
            LblRemplacementCompCD3.Text = CompJoueur[2].CD.ToString();
            LblRemplacementCompCD4.Text = CompJoueur[3].CD.ToString();
            LblRemplacementCompCD5.Text = CompJoueur[4].CD.ToString();
            LblRemplacementCompCD6.Text = CompJoueur[5].CD.ToString();
            LblRemplacementCompCD7.Text = CompJoueur[6].CD.ToString();
            LblRemplacementCompCD8.Text = CompJoueur[7].CD.ToString();
            LblRemplacementCompCDSup.Text = new_comp.CD.ToString();

            LblRemplacementCompDes1.Text = CompJoueur[0].Effet.ToString();
            LblRemplacementCompDes2.Text = CompJoueur[1].Effet.ToString();
            LblRemplacementCompDes3.Text = CompJoueur[2].Effet.ToString();
            LblRemplacementCompDes4.Text = CompJoueur[3].Effet.ToString();
            LblRemplacementCompDes5.Text = CompJoueur[4].Effet.ToString();
            LblRemplacementCompDes6.Text = CompJoueur[5].Effet.ToString();
            LblRemplacementCompDes7.Text = CompJoueur[6].Effet.ToString();
            LblRemplacementCompDes8.Text = CompJoueur[7].Effet.ToString();
            LblRemplacementCompDesSup.Text = new_comp.Effet.ToString();

            TxtBoxRemplacementComp.Text = "";
        }

        //Fonction actualisation Fonds Marché

        public void actualisationLblFondsMarché()
        {
            LblMarchéAchatVenteFonds.Text = "Fonds : " + Fonds.ToString();
            LblMarchéAchatChxTypeFonds.Text = "Fonds : " + Fonds.ToString();
            LblMarchéAchatChxRaretéFonds.Text = "Fonds : " + Fonds.ToString();
            LblMarchéAchatFonds.Text = "Fonds : " + Fonds.ToString();
            LblMarchéVenteFonds.Text = "Fonds : " + Fonds.ToString();
            LblMarchéAchatVenteEquipable.Text = "Equipable : " + ClassePerso.Equipable1;
            if (ClassePerso.Equipable2 != "")
            {
                LblMarchéAchatVenteEquipable.Text += "; " + ClassePerso.Equipable2;

                if (ClassePerso.Equipable3 != "")
                {
                    LblMarchéAchatVenteEquipable.Text += "; " + ClassePerso.Equipable3;

                    if (ClassePerso.Equipable4 != "")
                    {
                        LblMarchéAchatVenteEquipable.Text += "; " + ClassePerso.Equipable4;

                        if (ClassePerso.Equipable5 != "")
                        {
                            LblMarchéAchatVenteEquipable.Text += "; " + ClassePerso.Equipable5;

                            if (ClassePerso.Equipable6 != "")
                            {
                                LblMarchéAchatVenteEquipable.Text += "; " + ClassePerso.Equipable6;
                            }
                        }
                    }
                }
            }
        }

        //Fonction actualisation Objets Vente

        public void actualisationObjetsVente(ref ObjetInv[] inventairejoueur)
        {
            if (inventairejoueur[0].EmplOccupe)
            {
                BtnMarchéVenteObj1.Visible = true;
                LblMarchéVenteBonusObj1.Visible = true;
                LblMarchéVentePortéeObj1.Visible = true;
                LblMarchéVentePrixObj1.Visible = true;
                LblMarchéVenteEffetsSupObj1.Visible = true;

                BtnMarchéVenteObj1.Text = inventairejoueur[0].Nom;
                if (inventairejoueur[0].Type == "Soin" || inventairejoueur[0].Type == "Degats" || inventairejoueur[0].Type == "Buff" || inventairejoueur[0].Type == "Debuff")
                {
                    BtnMarchéVenteObj1.Text += " (" + inventairejoueur[0].Quantité.ToString() + ")";
                }

                LblMarchéVenteBonusObj1.Text = inventairejoueur[0].BonusType + " +" + inventairejoueur[0].Bonus.ToString();
                LblMarchéVentePortéeObj1.Text = inventairejoueur[0].Portee;
                LblMarchéVentePrixObj1.Text = (inventairejoueur[0].Prix / 2).ToString() + " G";

                if (inventairejoueur[0].EffetType1 != "")
                {
                    LblMarchéVenteEffetsSupObj1.Text = inventairejoueur[0].EffetType1 + ": " + inventairejoueur[0].EffetDes1;

                    if (inventairejoueur[0].EffetType2 != "")
                    {
                        LblMarchéVenteEffetsSupObj1.Text += " ; " + inventairejoueur[0].EffetType2 + ": " + inventairejoueur[0].EffetDes2;

                        if (inventairejoueur[0].EffetType3 != "")
                        {
                            LblMarchéVenteEffetsSupObj1.Text += " ; " + inventairejoueur[0].EffetType3 + ": " + inventairejoueur[0].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (inventairejoueur[0].Type == "Soin" || inventairejoueur[0].Type == "Degats" || inventairejoueur[0].Type == "Buff" || inventairejoueur[0].Type == "Debuff")
                    {
                        LblMarchéVenteEffetsSupObj1.Text = inventairejoueur[0].EffetDes1;
                        LblMarchéVenteBonusObj1.Text = "        /";
                    }
                    else
                    {
                        LblMarchéVenteEffetsSupObj1.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéVenteObj1.Visible = false;
                LblMarchéVenteBonusObj1.Visible = false;
                LblMarchéVentePortéeObj1.Visible = false;
                LblMarchéVentePrixObj1.Visible = false;
                LblMarchéVenteEffetsSupObj1.Visible = false;
            }

            if (inventairejoueur[1].EmplOccupe)
            {
                BtnMarchéVenteObj2.Visible = true;
                LblMarchéVenteBonusObj2.Visible = true;
                LblMarchéVentePortéeObj2.Visible = true;
                LblMarchéVentePrixObj2.Visible = true;
                LblMarchéVenteEffetsSupObj2.Visible = true;

                BtnMarchéVenteObj2.Text = inventairejoueur[1].Nom;
                if (inventairejoueur[1].Type == "Soin" || inventairejoueur[1].Type == "Degats" || inventairejoueur[1].Type == "Buff" || inventairejoueur[1].Type == "Debuff")
                {
                    BtnMarchéVenteObj2.Text += " (" + inventairejoueur[1].Quantité.ToString() + ")";
                }

                LblMarchéVenteBonusObj2.Text = inventairejoueur[1].BonusType + " +" + inventairejoueur[1].Bonus.ToString();
                LblMarchéVentePortéeObj2.Text = inventairejoueur[1].Portee;
                LblMarchéVentePrixObj2.Text = (inventairejoueur[1].Prix / 2).ToString() + " G";

                if (inventairejoueur[1].EffetType1 != "")
                {
                    LblMarchéVenteEffetsSupObj2.Text = inventairejoueur[1].EffetType1 + ": " + inventairejoueur[1].EffetDes1;

                    if (inventairejoueur[1].EffetType2 != "")
                    {
                        LblMarchéVenteEffetsSupObj2.Text += " ; " + inventairejoueur[1].EffetType2 + ": " + inventairejoueur[1].EffetDes2;

                        if (inventairejoueur[1].EffetType3 != "")
                        {
                            LblMarchéVenteEffetsSupObj2.Text += " ; " + inventairejoueur[1].EffetType3 + ": " + inventairejoueur[1].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (inventairejoueur[1].Type == "Soin" || inventairejoueur[1].Type == "Degats" || inventairejoueur[1].Type == "Buff" || inventairejoueur[1].Type == "Debuff")
                    {
                        LblMarchéVenteEffetsSupObj2.Text = inventairejoueur[1].EffetDes1;
                        LblMarchéVenteBonusObj2.Text = "        /";
                    }
                    else
                    {
                        LblMarchéVenteEffetsSupObj2.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéVenteObj2.Visible = false;
                LblMarchéVenteBonusObj2.Visible = false;
                LblMarchéVentePortéeObj2.Visible = false;
                LblMarchéVentePrixObj2.Visible = false;
                LblMarchéVenteEffetsSupObj2.Visible = false;
            }

            if (inventairejoueur[2].EmplOccupe)
            {
                BtnMarchéVenteObj3.Visible = true;
                LblMarchéVenteBonusObj3.Visible = true;
                LblMarchéVentePortéeObj3.Visible = true;
                LblMarchéVentePrixObj3.Visible = true;
                LblMarchéVenteEffetsSupObj3.Visible = true;

                BtnMarchéVenteObj3.Text = inventairejoueur[2].Nom;
                if (inventairejoueur[2].Type == "Soin" || inventairejoueur[2].Type == "Degats" || inventairejoueur[2].Type == "Buff" || inventairejoueur[2].Type == "Debuff")
                {
                    BtnMarchéVenteObj3.Text += " (" + inventairejoueur[2].Quantité.ToString() + ")";
                }

                LblMarchéVenteBonusObj3.Text = inventairejoueur[2].BonusType + " +" + inventairejoueur[2].Bonus.ToString();
                LblMarchéVentePortéeObj3.Text = inventairejoueur[2].Portee;
                LblMarchéVentePrixObj3.Text = (inventairejoueur[2].Prix / 2).ToString() + " G";

                if (inventairejoueur[2].EffetType1 != "")
                {
                    LblMarchéVenteEffetsSupObj3.Text = inventairejoueur[2].EffetType1 + ": " + inventairejoueur[2].EffetDes1;

                    if (inventairejoueur[2].EffetType2 != "")
                    {
                        LblMarchéVenteEffetsSupObj3.Text += " ; " + inventairejoueur[2].EffetType2 + ": " + inventairejoueur[2].EffetDes2;

                        if (inventairejoueur[2].EffetType3 != "")
                        {
                            LblMarchéVenteEffetsSupObj3.Text += " ; " + inventairejoueur[2].EffetType3 + ": " + inventairejoueur[2].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (inventairejoueur[2].Type == "Soin" || inventairejoueur[2].Type == "Degats" || inventairejoueur[2].Type == "Buff" || inventairejoueur[2].Type == "Debuff")
                    {
                        LblMarchéVenteEffetsSupObj3.Text = inventairejoueur[2].EffetDes1;
                        LblMarchéVenteBonusObj3.Text = "        /";
                    }
                    else
                    {
                        LblMarchéVenteEffetsSupObj3.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéVenteObj3.Visible = false;
                LblMarchéVenteBonusObj3.Visible = false;
                LblMarchéVentePortéeObj3.Visible = false;
                LblMarchéVentePrixObj3.Visible = false;
                LblMarchéVenteEffetsSupObj3.Visible = false;
            }

            if (inventairejoueur[3].EmplOccupe)
            {
                BtnMarchéVenteObj4.Visible = true;
                LblMarchéVenteBonusObj4.Visible = true;
                LblMarchéVentePortéeObj4.Visible = true;
                LblMarchéVentePrixObj4.Visible = true;
                LblMarchéVenteEffetsSupObj4.Visible = true;

                BtnMarchéVenteObj4.Text = inventairejoueur[3].Nom;
                if (inventairejoueur[3].Type == "Soin" || inventairejoueur[3].Type == "Degats" || inventairejoueur[3].Type == "Buff" || inventairejoueur[3].Type == "Debuff")
                {
                    BtnMarchéVenteObj4.Text += " (" + inventairejoueur[3].Quantité.ToString() + ")";
                }

                LblMarchéVenteBonusObj4.Text = inventairejoueur[3].BonusType + " +" + inventairejoueur[3].Bonus.ToString();
                LblMarchéVentePortéeObj4.Text = inventairejoueur[3].Portee;
                LblMarchéVentePrixObj4.Text = (inventairejoueur[3].Prix / 2).ToString() + " G";

                if (inventairejoueur[3].EffetType1 != "")
                {
                    LblMarchéVenteEffetsSupObj4.Text = inventairejoueur[3].EffetType1 + ": " + inventairejoueur[3].EffetDes1;

                    if (inventairejoueur[3].EffetType2 != "")
                    {
                        LblMarchéVenteEffetsSupObj4.Text += " ; " + inventairejoueur[3].EffetType2 + ": " + inventairejoueur[3].EffetDes2;

                        if (inventairejoueur[3].EffetType3 != "")
                        {
                            LblMarchéVenteEffetsSupObj4.Text += " ; " + inventairejoueur[3].EffetType3 + ": " + inventairejoueur[3].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (inventairejoueur[3].Type == "Soin" || inventairejoueur[3].Type == "Degats" || inventairejoueur[3].Type == "Buff" || inventairejoueur[3].Type == "Debuff")
                    {
                        LblMarchéVenteEffetsSupObj4.Text = inventairejoueur[3].EffetDes1;
                        LblMarchéVenteBonusObj4.Text = "        /";
                    }
                    else
                    {
                        LblMarchéVenteEffetsSupObj4.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéVenteObj4.Visible = false;
                LblMarchéVenteBonusObj4.Visible = false;
                LblMarchéVentePortéeObj4.Visible = false;
                LblMarchéVentePrixObj4.Visible = false;
                LblMarchéVenteEffetsSupObj4.Visible = false;
            }

            if (inventairejoueur[4].EmplOccupe)
            {
                BtnMarchéVenteObj5.Visible = true;
                LblMarchéVenteBonusObj5.Visible = true;
                LblMarchéVentePortéeObj5.Visible = true;
                LblMarchéVentePrixObj5.Visible = true;
                LblMarchéVenteEffetsSupObj5.Visible = true;

                BtnMarchéVenteObj5.Text = inventairejoueur[4].Nom;
                if (inventairejoueur[4].Type == "Soin" || inventairejoueur[4].Type == "Degats" || inventairejoueur[4].Type == "Buff" || inventairejoueur[4].Type == "Debuff")
                {
                    BtnMarchéVenteObj5.Text += " (" + inventairejoueur[4].Quantité.ToString() + ")";
                }

                LblMarchéVenteBonusObj5.Text = inventairejoueur[4].BonusType + " +" + inventairejoueur[4].Bonus.ToString();
                LblMarchéVentePortéeObj5.Text = inventairejoueur[4].Portee;
                LblMarchéVentePrixObj5.Text = (inventairejoueur[4].Prix / 2).ToString() + " G";

                if (inventairejoueur[4].EffetType1 != "")
                {
                    LblMarchéVenteEffetsSupObj5.Text = inventairejoueur[4].EffetType1 + ": " + inventairejoueur[4].EffetDes1;

                    if (inventairejoueur[4].EffetType2 != "")
                    {
                        LblMarchéVenteEffetsSupObj5.Text += " ; " + inventairejoueur[4].EffetType2 + ": " + inventairejoueur[4].EffetDes2;

                        if (inventairejoueur[4].EffetType3 != "")
                        {
                            LblMarchéVenteEffetsSupObj5.Text += " ; " + inventairejoueur[4].EffetType3 + ": " + inventairejoueur[4].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (inventairejoueur[4].Type == "Soin" || inventairejoueur[4].Type == "Degats" || inventairejoueur[4].Type == "Buff" || inventairejoueur[4].Type == "Debuff")
                    {
                        LblMarchéVenteEffetsSupObj5.Text = inventairejoueur[4].EffetDes1;
                        LblMarchéVenteBonusObj5.Text = "        /";
                    }
                    else
                    {
                        LblMarchéVenteEffetsSupObj5.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéVenteObj5.Visible = false;
                LblMarchéVenteBonusObj5.Visible = false;
                LblMarchéVentePortéeObj5.Visible = false;
                LblMarchéVentePrixObj5.Visible = false;
                LblMarchéVenteEffetsSupObj5.Visible = false;
            }

            if (inventairejoueur[5].EmplOccupe)
            {
                BtnMarchéVenteObj6.Visible = true;
                LblMarchéVenteBonusObj6.Visible = true;
                LblMarchéVentePortéeObj6.Visible = true;
                LblMarchéVentePrixObj6.Visible = true;
                LblMarchéVenteEffetsSupObj6.Visible = true;

                BtnMarchéVenteObj6.Text = inventairejoueur[5].Nom;
                if (inventairejoueur[5].Type == "Soin" || inventairejoueur[5].Type == "Degats" || inventairejoueur[5].Type == "Buff" || inventairejoueur[5].Type == "Debuff")
                {
                    BtnMarchéVenteObj6.Text += " (" + inventairejoueur[5].Quantité.ToString() + ")";
                }

                LblMarchéVenteBonusObj6.Text = inventairejoueur[5].BonusType + " +" + inventairejoueur[5].Bonus.ToString();
                LblMarchéVentePortéeObj6.Text = inventairejoueur[5].Portee;
                LblMarchéVentePrixObj6.Text = (inventairejoueur[5].Prix / 2).ToString() + " G";

                if (inventairejoueur[5].EffetType1 != "")
                {
                    LblMarchéVenteEffetsSupObj6.Text = inventairejoueur[5].EffetType1 + ": " + inventairejoueur[5].EffetDes1;

                    if (inventairejoueur[5].EffetType2 != "")
                    {
                        LblMarchéVenteEffetsSupObj6.Text += " ; " + inventairejoueur[5].EffetType2 + ": " + inventairejoueur[5].EffetDes2;

                        if (inventairejoueur[5].EffetType3 != "")
                        {
                            LblMarchéVenteEffetsSupObj6.Text += " ; " + inventairejoueur[5].EffetType3 + ": " + inventairejoueur[5].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (inventairejoueur[5].Type == "Soin" || inventairejoueur[5].Type == "Degats" || inventairejoueur[5].Type == "Buff" || inventairejoueur[5].Type == "Debuff")
                    {
                        LblMarchéVenteEffetsSupObj6.Text = inventairejoueur[5].EffetDes1;
                        LblMarchéVenteBonusObj6.Text = "        /";
                    }
                    else
                    {
                        LblMarchéVenteEffetsSupObj6.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéVenteObj6.Visible = false;
                LblMarchéVenteBonusObj6.Visible = false;
                LblMarchéVentePortéeObj6.Visible = false;
                LblMarchéVentePrixObj6.Visible = false;
                LblMarchéVenteEffetsSupObj6.Visible = false;
            }

            if (inventairejoueur[6].EmplOccupe)
            {
                BtnMarchéVenteObj7.Visible = true;
                LblMarchéVenteBonusObj7.Visible = true;
                LblMarchéVentePortéeObj7.Visible = true;
                LblMarchéVentePrixObj7.Visible = true;
                LblMarchéVenteEffetsSupObj7.Visible = true;

                BtnMarchéVenteObj7.Text = inventairejoueur[6].Nom;
                if (inventairejoueur[6].Type == "Soin" || inventairejoueur[6].Type == "Degats" || inventairejoueur[6].Type == "Buff" || inventairejoueur[6].Type == "Debuff")
                {
                    BtnMarchéVenteObj7.Text += " (" + inventairejoueur[6].Quantité.ToString() + ")";
                }

                LblMarchéVenteBonusObj7.Text = inventairejoueur[6].BonusType + " +" + inventairejoueur[6].Bonus.ToString();
                LblMarchéVentePortéeObj7.Text = inventairejoueur[6].Portee;
                LblMarchéVentePrixObj7.Text = (inventairejoueur[6].Prix / 2).ToString() + " G";

                if (inventairejoueur[6].EffetType1 != "")
                {
                    LblMarchéVenteEffetsSupObj7.Text = inventairejoueur[6].EffetType1 + ": " + inventairejoueur[6].EffetDes1;

                    if (inventairejoueur[6].EffetType2 != "")
                    {
                        LblMarchéVenteEffetsSupObj7.Text += " ; " + inventairejoueur[6].EffetType2 + ": " + inventairejoueur[6].EffetDes2;

                        if (inventairejoueur[6].EffetType3 != "")
                        {
                            LblMarchéVenteEffetsSupObj7.Text += " ; " + inventairejoueur[6].EffetType3 + ": " + inventairejoueur[6].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (inventairejoueur[6].Type == "Soin" || inventairejoueur[6].Type == "Degats" || inventairejoueur[6].Type == "Buff" || inventairejoueur[6].Type == "Debuff")
                    {
                        LblMarchéVenteEffetsSupObj7.Text = inventairejoueur[6].EffetDes1;
                        LblMarchéVenteBonusObj7.Text = "        /";
                    }
                    else
                    {
                        LblMarchéVenteEffetsSupObj7.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéVenteObj7.Visible = false;
                LblMarchéVenteBonusObj7.Visible = false;
                LblMarchéVentePortéeObj7.Visible = false;
                LblMarchéVentePrixObj7.Visible = false;
                LblMarchéVenteEffetsSupObj7.Visible = false;
            }

            if (inventairejoueur[7].EmplOccupe)
            {
                BtnMarchéVenteObj8.Visible = true;
                LblMarchéVenteBonusObj8.Visible = true;
                LblMarchéVentePortéeObj8.Visible = true;
                LblMarchéVentePrixObj8.Visible = true;
                LblMarchéVenteEffetsSupObj8.Visible = true;

                BtnMarchéVenteObj8.Text = inventairejoueur[7].Nom;
                if (inventairejoueur[7].Type == "Soin" || inventairejoueur[7].Type == "Degats" || inventairejoueur[7].Type == "Buff" || inventairejoueur[7].Type == "Debuff")
                {
                    BtnMarchéVenteObj8.Text += " (" + inventairejoueur[7].Quantité.ToString() + ")";
                }

                LblMarchéVenteBonusObj8.Text = inventairejoueur[7].BonusType + " +" + inventairejoueur[7].Bonus.ToString();
                LblMarchéVentePortéeObj8.Text = inventairejoueur[7].Portee;
                LblMarchéVentePrixObj8.Text = (inventairejoueur[7].Prix / 2).ToString() + " G";

                if (inventairejoueur[7].EffetType1 != "")
                {
                    LblMarchéVenteEffetsSupObj8.Text = inventairejoueur[7].EffetType1 + ": " + inventairejoueur[7].EffetDes1;

                    if (inventairejoueur[7].EffetType2 != "")
                    {
                        LblMarchéVenteEffetsSupObj8.Text += " ; " + inventairejoueur[7].EffetType2 + ": " + inventairejoueur[7].EffetDes2;

                        if (inventairejoueur[7].EffetType3 != "")
                        {
                            LblMarchéVenteEffetsSupObj8.Text += " ; " + inventairejoueur[7].EffetType3 + ": " + inventairejoueur[7].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (inventairejoueur[7].Type == "Soin" || inventairejoueur[7].Type == "Degats" || inventairejoueur[7].Type == "Buff" || inventairejoueur[7].Type == "Debuff")
                    {
                        LblMarchéVenteEffetsSupObj8.Text = inventairejoueur[7].EffetDes1;
                        LblMarchéVenteBonusObj8.Text = "        /";
                    }
                    else
                    {
                        LblMarchéVenteEffetsSupObj8.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéVenteObj8.Visible = false;
                LblMarchéVenteBonusObj8.Visible = false;
                LblMarchéVentePortéeObj8.Visible = false;
                LblMarchéVentePrixObj8.Visible = false;
                LblMarchéVenteEffetsSupObj8.Visible = false;
            }
        }

        //Fonction actualisation Objets Achat

        public void actualisationObjetsAchat(ref ObjetInv[] objmarché, ref ObjetInv[] objinventaire, string type, string rareté)
        {
            int i = 0;
            int j = 0;

            while (i < 6)
            {
                objmarché[i].Type = "";
                objmarché[i].Nom = "";
                objmarché[i].Rarete = "";
                objmarché[i].Portee = "";
                objmarché[i].BonusType = "";
                objmarché[i].Bonus = 0;
                objmarché[i].EffetType1 = "";
                objmarché[i].EffetDes1 = "";
                objmarché[i].EffetType2 = "";
                objmarché[i].EffetDes2 = "";
                objmarché[i].EffetType3 = "";
                objmarché[i].EffetDes3 = "";
                objmarché[i].Quantité = 0;
                objmarché[i].Prix = 0;
                objmarché[i].EmplOccupe = false;
                i++;
            }

            i = 0;

            while ((j < 411) && (i < 6))
            {
                if ((objinventaire[j].Type == type) && objinventaire[j].Rarete == rareté)
                {
                    objmarché[i] = objinventaire[j];
                    i++;
                }
                j++;
            }

            if (i >= 1)
            {
                BtnMarchéAchatObj1.Visible = true;
                LblMarchéAchatBonusObj1.Visible = true;
                LblMarchéAchatPortéeObj1.Visible = true;
                LblMarchéAchatPrixObj1.Visible = true;
                LblMarchéAchatEffetsSupObj1.Visible = true;

                BtnMarchéAchatObj1.Text = objmarché[0].Nom.ToString();
                LblMarchéAchatBonusObj1.Text = objmarché[0].BonusType + " +" + objmarché[0].Bonus.ToString();
                LblMarchéAchatPortéeObj1.Text = objmarché[0].Portee;
                LblMarchéAchatPrixObj1.Text = objmarché[0].Prix.ToString() + " G";

                if (objmarché[0].EffetType1 != "")
                {
                    LblMarchéAchatEffetsSupObj1.Text = objmarché[0].EffetType1 + ": " + objmarché[0].EffetDes1;

                    if (objmarché[0].EffetType2 != "")
                    {
                        LblMarchéAchatEffetsSupObj1.Text += " ; " + objmarché[0].EffetType2 + ": " + objmarché[0].EffetDes2;

                        if (objmarché[0].EffetType3 != "")
                        {
                            LblMarchéAchatEffetsSupObj1.Text += " ; " + objmarché[0].EffetType3 + ": " + objmarché[0].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (objmarché[0].Type == "Soin" || objmarché[0].Type == "Degats" || objmarché[0].Type == "Buff" || objmarché[0].Type == "Debuff" || objmarché[0].Type == "Comp")
                    {
                        LblMarchéAchatEffetsSupObj1.Text = objmarché[0].EffetDes1;
                        LblMarchéAchatBonusObj1.Text = "        /";
                    }
                    else
                    {
                        LblMarchéAchatEffetsSupObj1.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéAchatObj1.Visible = false;
                LblMarchéAchatBonusObj1.Visible = false;
                LblMarchéAchatPortéeObj1.Visible = false;
                LblMarchéAchatPrixObj1.Visible = false;
                LblMarchéAchatEffetsSupObj1.Visible = false;
            }

            if (i >= 2)
            {
                BtnMarchéAchatObj2.Visible = true;
                LblMarchéAchatBonusObj2.Visible = true;
                LblMarchéAchatPortéeObj2.Visible = true;
                LblMarchéAchatPrixObj2.Visible = true;
                LblMarchéAchatEffetsSupObj2.Visible = true;

                BtnMarchéAchatObj2.Text = objmarché[1].Nom.ToString();
                LblMarchéAchatBonusObj2.Text = objmarché[1].BonusType + " +" + objmarché[1].Bonus.ToString();
                LblMarchéAchatPortéeObj2.Text = objmarché[1].Portee;
                LblMarchéAchatPrixObj2.Text = objmarché[1].Prix.ToString() + " G";

                if (objmarché[1].EffetType1 != "")
                {
                    LblMarchéAchatEffetsSupObj2.Text = objmarché[1].EffetType1 + ": " + objmarché[1].EffetDes1;

                    if (objmarché[1].EffetType2 != "")
                    {
                        LblMarchéAchatEffetsSupObj2.Text += " ; " + objmarché[1].EffetType2 + ": " + objmarché[1].EffetDes2;

                        if (objmarché[1].EffetType3 != "")
                        {
                            LblMarchéAchatEffetsSupObj2.Text += " ; " + objmarché[1].EffetType3 + ": " + objmarché[1].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (objmarché[1].Type == "Soin" || objmarché[1].Type == "Degats" || objmarché[1].Type == "Buff" || objmarché[1].Type == "Debuff" || objmarché[1].Type == "Comp")
                    {
                        LblMarchéAchatEffetsSupObj2.Text = objmarché[1].EffetDes1;
                        LblMarchéAchatBonusObj2.Text = "        /";
                    }
                    else
                    {
                        LblMarchéAchatEffetsSupObj2.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéAchatObj2.Visible = false;
                LblMarchéAchatBonusObj2.Visible = false;
                LblMarchéAchatPortéeObj2.Visible = false;
                LblMarchéAchatPrixObj2.Visible = false;
                LblMarchéAchatEffetsSupObj2.Visible = false;
            }

            if (i >= 3)
            {
                BtnMarchéAchatObj3.Visible = true;
                LblMarchéAchatBonusObj3.Visible = true;
                LblMarchéAchatPortéeObj3.Visible = true;
                LblMarchéAchatPrixObj3.Visible = true;
                LblMarchéAchatEffetsSupObj3.Visible = true;

                BtnMarchéAchatObj3.Text = objmarché[2].Nom.ToString();
                LblMarchéAchatBonusObj3.Text = objmarché[2].BonusType + " +" + objmarché[2].Bonus.ToString();
                LblMarchéAchatPortéeObj3.Text = objmarché[2].Portee;
                LblMarchéAchatPrixObj3.Text = objmarché[2].Prix.ToString() + " G";

                if (objmarché[2].EffetType1 != "")
                {
                    LblMarchéAchatEffetsSupObj3.Text = objmarché[2].EffetType1 + ": " + objmarché[2].EffetDes1;

                    if (objmarché[2].EffetType2 != "")
                    {
                        LblMarchéAchatEffetsSupObj3.Text += " ; " + objmarché[2].EffetType2 + ": " + objmarché[2].EffetDes2;

                        if (objmarché[2].EffetType3 != "")
                        {
                            LblMarchéAchatEffetsSupObj3.Text += " ; " + objmarché[2].EffetType3 + ": " + objmarché[2].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (objmarché[2].Type == "Soin" || objmarché[2].Type == "Degats" || objmarché[2].Type == "Buff" || objmarché[2].Type == "Debuff" || objmarché[2].Type == "Comp")
                    {
                        LblMarchéAchatEffetsSupObj3.Text = objmarché[2].EffetDes1;
                        LblMarchéAchatBonusObj3.Text = "        /";
                    }
                    else
                    {
                        LblMarchéAchatEffetsSupObj3.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéAchatObj3.Visible = false;
                LblMarchéAchatBonusObj3.Visible = false;
                LblMarchéAchatPortéeObj3.Visible = false;
                LblMarchéAchatPrixObj3.Visible = false;
                LblMarchéAchatEffetsSupObj3.Visible = false;
            }

            if (i >= 4)
            {
                BtnMarchéAchatObj4.Visible = true;
                LblMarchéAchatBonusObj4.Visible = true;
                LblMarchéAchatPortéeObj4.Visible = true;
                LblMarchéAchatPrixObj4.Visible = true;
                LblMarchéAchatEffetsSupObj4.Visible = true;

                BtnMarchéAchatObj4.Text = objmarché[3].Nom.ToString();
                LblMarchéAchatBonusObj4.Text = objmarché[3].BonusType + " +" + objmarché[3].Bonus.ToString();
                LblMarchéAchatPortéeObj4.Text = objmarché[3].Portee;
                LblMarchéAchatPrixObj4.Text = objmarché[3].Prix.ToString() + " G";

                if (objmarché[3].EffetType1 != "")
                {
                    LblMarchéAchatEffetsSupObj4.Text = objmarché[3].EffetType1 + ": " + objmarché[3].EffetDes1;

                    if (objmarché[3].EffetType2 != "")
                    {
                        LblMarchéAchatEffetsSupObj4.Text += " ; " + objmarché[3].EffetType2 + ": " + objmarché[3].EffetDes2;

                        if (objmarché[3].EffetType3 != "")
                        {
                            LblMarchéAchatEffetsSupObj4.Text += " ; " + objmarché[3].EffetType3 + ": " + objmarché[3].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (objmarché[3].Type == "Soin" || objmarché[3].Type == "Degats" || objmarché[3].Type == "Buff" || objmarché[3].Type == "Debuff" || objmarché[3].Type == "Comp")
                    {
                        LblMarchéAchatEffetsSupObj4.Text = objmarché[3].EffetDes1;
                        LblMarchéAchatBonusObj4.Text = "        /";
                    }
                    else
                    {
                        LblMarchéAchatEffetsSupObj4.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéAchatObj4.Visible = false;
                LblMarchéAchatBonusObj4.Visible = false;
                LblMarchéAchatPortéeObj4.Visible = false;
                LblMarchéAchatPrixObj4.Visible = false;
                LblMarchéAchatEffetsSupObj4.Visible = false;
            }

            if (i >= 5)
            {
                BtnMarchéAchatObj5.Visible = true;
                LblMarchéAchatBonusObj5.Visible = true;
                LblMarchéAchatPortéeObj5.Visible = true;
                LblMarchéAchatPrixObj5.Visible = true;
                LblMarchéAchatEffetsSupObj5.Visible = true;

                BtnMarchéAchatObj5.Text = objmarché[4].Nom.ToString();
                LblMarchéAchatBonusObj5.Text = objmarché[4].BonusType + " +" + objmarché[4].Bonus.ToString();
                LblMarchéAchatPortéeObj5.Text = objmarché[4].Portee;
                LblMarchéAchatPrixObj5.Text = objmarché[4].Prix.ToString() + " G";

                if (objmarché[4].EffetType1 != "")
                {
                    LblMarchéAchatEffetsSupObj5.Text = objmarché[4].EffetType1 + ": " + objmarché[4].EffetDes1;

                    if (objmarché[4].EffetType2 != "")
                    {
                        LblMarchéAchatEffetsSupObj5.Text += " ; " + objmarché[4].EffetType2 + ": " + objmarché[4].EffetDes2;

                        if (objmarché[4].EffetType3 != "")
                        {
                            LblMarchéAchatEffetsSupObj5.Text += " ; " + objmarché[4].EffetType3 + ": " + objmarché[4].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (objmarché[4].Type == "Soin" || objmarché[4].Type == "Degats" || objmarché[4].Type == "Buff" || objmarché[4].Type == "Debuff" || objmarché[4].Type == "Comp")
                    {
                        LblMarchéAchatEffetsSupObj5.Text = objmarché[4].EffetDes1;
                        LblMarchéAchatBonusObj5.Text = "        /";
                    }
                    else
                    {
                        LblMarchéAchatEffetsSupObj5.Text = "";
                    }
                }

            }
            else
            {
                BtnMarchéAchatObj5.Visible = false;
                LblMarchéAchatBonusObj5.Visible = false;
                LblMarchéAchatPortéeObj5.Visible = false;
                LblMarchéAchatPrixObj5.Visible = false;
                LblMarchéAchatEffetsSupObj5.Visible = false;
            }

            if (i == 6)
            {
                BtnMarchéAchatObj6.Visible = true;
                LblMarchéAchatBonusObj6.Visible = true;
                LblMarchéAchatPortéeObj6.Visible = true;
                LblMarchéAchatPrixObj6.Visible = true;
                LblMarchéAchatEffetsSupObj6.Visible = true;

                BtnMarchéAchatObj6.Text = objmarché[5].Nom.ToString();
                LblMarchéAchatBonusObj6.Text = objmarché[5].BonusType + " +" + objmarché[5].Bonus.ToString();
                LblMarchéAchatPortéeObj6.Text = objmarché[5].Portee;
                LblMarchéAchatPrixObj6.Text = objmarché[5].Prix.ToString() + " G";

                if (objmarché[5].EffetType1 != "")
                {
                    LblMarchéAchatEffetsSupObj6.Text = objmarché[5].EffetType1 + ": " + objmarché[5].EffetDes1;

                    if (objmarché[5].EffetType2 != "")
                    {
                        LblMarchéAchatEffetsSupObj6.Text += " ; " + objmarché[5].EffetType2 + ": " + objmarché[5].EffetDes2;

                        if (objmarché[5].EffetType3 != "")
                        {
                            LblMarchéAchatEffetsSupObj6.Text += " ; " + objmarché[5].EffetType3 + ": " + objmarché[5].EffetDes3;
                        }
                    }
                }
                else
                {
                    if (objmarché[5].Type == "Soin" || objmarché[5].Type == "Degats" || objmarché[5].Type == "Buff" || objmarché[5].Type == "Debuff" || objmarché[5].Type == "Comp")
                    {
                        LblMarchéAchatEffetsSupObj6.Text = objmarché[5].EffetDes1;
                        LblMarchéAchatBonusObj6.Text = "        /";
                    }
                    else
                    {
                        LblMarchéAchatEffetsSupObj6.Text = "";
                    }
                }
            }
            else
            {
                BtnMarchéAchatObj6.Visible = false;
                LblMarchéAchatBonusObj6.Visible = false;
                LblMarchéAchatPortéeObj6.Visible = false;
                LblMarchéAchatPrixObj6.Visible = false;
                LblMarchéAchatEffetsSupObj6.Visible = false;
            }
        }

        //Fonction actualisation Objets Alchimie

        public void actualisationObjetsAlchimie(ref ObjetInv[] objmarché, ref ObjetInv[] objinventaire, string type, string rareté)
        {
            int i = 0;
            int j = 0;

            while (i < 6)
            {
                objmarché[i].Type = "";
                objmarché[i].Nom = "";
                objmarché[i].Rarete = "";
                objmarché[i].Portee = "";
                objmarché[i].BonusType = "";
                objmarché[i].Bonus = 0;
                objmarché[i].EffetType1 = "";
                objmarché[i].EffetDes1 = "";
                objmarché[i].EffetType2 = "";
                objmarché[i].EffetDes2 = "";
                objmarché[i].EffetType3 = "";
                objmarché[i].EffetDes3 = "";
                objmarché[i].Quantité = 0;
                objmarché[i].Prix = 0;
                objmarché[i].EmplOccupe = false;
                i++;
            }

            i = 0;

            while ((j < 411) && (i < 6))
            {
                if ((objinventaire[j].Type == type) && objinventaire[j].Rarete == rareté)
                {
                    objmarché[i] = objinventaire[j];
                    i++;
                }
                j++;
            }

            if (i >= 1)
            {
                BtnCompAlchimie3Obj1.Visible = true;
                LblCompAlchimie3PorteeObj1.Visible = true;
                LblCompAlchimie3EffetsObj1.Visible = true;

                BtnCompAlchimie3Obj1.Text = objmarché[0].Nom.ToString();
                LblCompAlchimie3PorteeObj1.Text = objmarché[0].Portee;
                LblCompAlchimie3EffetsObj1.Text = objmarché[0].EffetDes1;
            }
            else
            {
                BtnCompAlchimie3Obj1.Visible = false;
                LblCompAlchimie3PorteeObj1.Visible = false;
                LblCompAlchimie3EffetsObj1.Visible = false;
            }

            if (i >= 2)
            {
                BtnCompAlchimie3Obj2.Visible = true;
                LblCompAlchimie3PorteeObj2.Visible = true;
                LblCompAlchimie3EffetsObj2.Visible = true;

                BtnCompAlchimie3Obj2.Text = objmarché[1].Nom.ToString();
                LblCompAlchimie3PorteeObj2.Text = objmarché[1].Portee;
                LblCompAlchimie3EffetsObj2.Text = objmarché[1].EffetDes1;
            }
            else
            {
                BtnCompAlchimie3Obj2.Visible = false;
                LblCompAlchimie3PorteeObj2.Visible = false;
                LblCompAlchimie3EffetsObj2.Visible = false;
            }

            if (i >= 3)
            {
                BtnCompAlchimie3Obj3.Visible = true;
                LblCompAlchimie3PorteeObj3.Visible = true;
                LblCompAlchimie3EffetsObj3.Visible = true;

                BtnCompAlchimie3Obj3.Text = objmarché[2].Nom.ToString();
                LblCompAlchimie3PorteeObj3.Text = objmarché[2].Portee;
                LblCompAlchimie3EffetsObj3.Text = objmarché[2].EffetDes1;
            }
            else
            {
                BtnCompAlchimie3Obj3.Visible = false;
                LblCompAlchimie3PorteeObj3.Visible = false;
                LblCompAlchimie3EffetsObj3.Visible = false;
            }

            if (i >= 4)
            {
                BtnCompAlchimie3Obj4.Visible = true;
                LblCompAlchimie3PorteeObj4.Visible = true;
                LblCompAlchimie3EffetsObj4.Visible = true;

                BtnCompAlchimie3Obj4.Text = objmarché[3].Nom.ToString();
                LblCompAlchimie3PorteeObj4.Text = objmarché[3].Portee;
                LblCompAlchimie3EffetsObj4.Text = objmarché[3].EffetDes1;
            }
            else
            {
                BtnCompAlchimie3Obj4.Visible = false;
                LblCompAlchimie3PorteeObj4.Visible = false;
                LblCompAlchimie3EffetsObj4.Visible = false;
            }

            if (i >= 5)
            {
                BtnCompAlchimie3Obj5.Visible = true;
                LblCompAlchimie3PorteeObj5.Visible = true;
                LblCompAlchimie3EffetsObj5.Visible = true;

                BtnCompAlchimie3Obj5.Text = objmarché[4].Nom.ToString();
                LblCompAlchimie3PorteeObj5.Text = objmarché[4].Portee;
                LblCompAlchimie3EffetsObj5.Text = objmarché[4].EffetDes1;
            }
            else
            {
                BtnCompAlchimie3Obj5.Visible = false;
                LblCompAlchimie3PorteeObj5.Visible = false;
                LblCompAlchimie3EffetsObj5.Visible = false;
            }

            if (i == 6)
            {
                BtnCompAlchimie3Obj6.Visible = true;
                LblCompAlchimie3PorteeObj6.Visible = true;
                LblCompAlchimie3EffetsObj6.Visible = true;

                BtnCompAlchimie3Obj6.Text = objmarché[5].Nom.ToString();
                LblCompAlchimie3PorteeObj6.Text = objmarché[5].Portee;
                LblCompAlchimie3EffetsObj6.Text = objmarché[5].EffetDes1;
            }
            else
            {
                BtnCompAlchimie3Obj6.Visible = false;
                LblCompAlchimie3PorteeObj6.Visible = false;
                LblCompAlchimie3EffetsObj6.Visible = false;
            }
        }

        //Fonction apparition Boutons Phase

        public void apparitionBtnPhase()
        {
            if (Phase == "d'enquete")
            {
                BtnInfosEnnemis.Visible = false;
                BtnEffetRecu.Visible = false;
                BtnFinTour.Visible = false;
                BtnPlanificationActivité.Visible = true;
                BtnFinPériode.Visible = true;
            }
            else
            {
                BtnInfosEnnemis.Visible = true;
                BtnEffetRecu.Visible = true;
                BtnFinTour.Visible = true;
                BtnPlanificationActivité.Visible = false;
                BtnFinPériode.Visible = false;
            }
        }

        //Fonction initialisation classe

        public void initClasse(string ChxClasse, ref Classe ClassPers, ref ObjetInv[] inventairejoueur)
        {
            int i = 0;

            switch (ChxClasse)
            {
                case "Vagabond":
                    ClassPers.PVGrowth = 1.5;
                    ClassPers.MvtBase = "M";
                    ClassPers.Dégâts = "1D6";
                    ClassPers.PreBase = 60;
                    ClassPers.EsqBase = 40;
                    ClassPers.CritBase = 6;
                    ClassPers.VitGrowth = 1.75;
                    ClassPers.ConsGrowth = 2;
                    ClassPers.IntGrowth = 0.5;
                    ClassPers.ChaGrowth = 1;
                    ClassPers.SauGrowth = 1.25;
                    ClassPers.InsGrowth = 1.25;
                    ClassPers.Equipable1 = "Epee";
                    ClassPers.Equipable2 = "Pugilat";

                    while (Competences[i].Nom != "Errance")
                        i++;

                    CompJoueur[0] = Competences[i];

                    LblCreationFicheSocial.Text = "Lancez 1D100 - 20 pour la carac de Social";
                    break;

                case "Combattant":
                    ClassPers.PVGrowth = 1.25;
                    ClassPers.MvtBase = "M";
                    ClassPers.Dégâts = "1D8";
                    ClassPers.PreBase = 40;
                    ClassPers.EsqBase = 40;
                    ClassPers.CritBase = 10;
                    ClassPers.VitGrowth = 0.5;
                    ClassPers.ConsGrowth = 1.5;
                    ClassPers.IntGrowth = 1.5;
                    ClassPers.ChaGrowth = 0.75;
                    ClassPers.SauGrowth = 1.75;
                    ClassPers.InsGrowth = 2;
                    ClassPers.Equipable1 = "Arc";
                    ClassPers.Equipable2 = "Hache";

                    while (Competences[i].Nom != "Gargarisation")
                        i++;

                    CompJoueur[0] = Competences[i];

                    LblCreationFicheSocial.Text = "Lancez 1D100 pour la carac de Social";
                    break;

                case "Maso":
                    ClassPers.PVGrowth = 2;
                    ClassPers.MvtBase = "S";
                    ClassPers.Dégâts = "1D6";
                    ClassPers.PreBase = 40;
                    ClassPers.EsqBase = 20;
                    ClassPers.CritBase = 8;
                    ClassPers.VitGrowth = 0.75;
                    ClassPers.ConsGrowth = 1.75;
                    ClassPers.IntGrowth = 1.25;
                    ClassPers.ChaGrowth = 1.5;
                    ClassPers.SauGrowth = 1.25;
                    ClassPers.InsGrowth = 1.5;
                    ClassPers.Equipable1 = "Lancer";
                    ClassPers.Equipable2 = "Bouclier";

                    while (Competences[i].Nom != "Moitie-moitie")
                        i++;

                    CompJoueur[0] = Competences[i];

                    LblCreationFicheSocial.Text = "Lancez 1D100 - 20 pour la carac de Social";
                    break;

                case "Arcaniste":
                    ClassPers.PVGrowth = 0.75;
                    ClassPers.MvtBase = "S";
                    ClassPers.Dégâts = "1D6";
                    ClassPers.PreBase = 50;
                    ClassPers.EsqBase = 50;
                    ClassPers.CritBase = 12;
                    ClassPers.VitGrowth = 1;
                    ClassPers.ConsGrowth = 0.5;
                    ClassPers.IntGrowth = 2;
                    ClassPers.ChaGrowth = 1.75;
                    ClassPers.SauGrowth = 1.5;
                    ClassPers.InsGrowth = 1;
                    ClassPers.Equipable1 = "Tome";
                    ClassPers.Equipable2 = "Bestipierre";

                    while (Competences[i].Nom != "Effluves nefastes")
                        i++;

                    CompJoueur[0] = Competences[i];

                    LblCreationFicheSocial.Text = "Lancez 1D100 - 10 pour la carac de Social";
                    break;

                case "Prestidigitateur":
                    ClassPers.PVGrowth = 1;
                    ClassPers.MvtBase = "M";
                    ClassPers.Dégâts = "1D6";
                    ClassPers.PreBase = 80;
                    ClassPers.EsqBase = 60;
                    ClassPers.CritBase = 10;
                    ClassPers.VitGrowth = 1.5;
                    ClassPers.ConsGrowth = 0.75;
                    ClassPers.IntGrowth = 1.75;
                    ClassPers.ChaGrowth = 1.25;
                    ClassPers.SauGrowth = 1;
                    ClassPers.InsGrowth = 0.5;
                    ClassPers.Equipable1 = "Outil";
                    ClassPers.Equipable2 = "Fronde";

                    while (Competences[i].Nom != "Tada !")
                        i++;

                    CompJoueur[0] = Competences[i];

                    LblCreationFicheSocial.Text = "Lancez 1D100 + 10 pour la carac de Social";
                    break;

                case "Serviteur":
                    ClassPers.PVGrowth = 1.25;
                    ClassPers.MvtBase = "M";
                    ClassPers.Dégâts = "1D6";
                    ClassPers.PreBase = 70;
                    ClassPers.EsqBase = 60;
                    ClassPers.CritBase = 12;
                    ClassPers.VitGrowth = 2;
                    ClassPers.ConsGrowth = 1.25;
                    ClassPers.IntGrowth = 1;
                    ClassPers.ChaGrowth = 0.5;
                    ClassPers.SauGrowth = 1;
                    ClassPers.InsGrowth = 0.75;
                    ClassPers.Equipable1 = "Dague";
                    ClassPers.Equipable2 = "";

                    while (Competences[i].Nom != "Subordonne")
                        i++;

                    CompJoueur[0] = Competences[i];

                    LblCreationFicheSocial.Text = "Lancez 1D100 - 10 pour la carac de Social";
                    break;

                case "Infirmier":
                    ClassPers.PVGrowth = 0.5;
                    ClassPers.MvtBase = "M";
                    ClassPers.Dégâts = "1D6";
                    ClassPers.PreBase = 60;
                    ClassPers.EsqBase = 80;
                    ClassPers.CritBase = 8;
                    ClassPers.VitGrowth = 1.25;
                    ClassPers.ConsGrowth = 1;
                    ClassPers.IntGrowth = 1.5;
                    ClassPers.ChaGrowth = 1.25;
                    ClassPers.SauGrowth = 0.75;
                    ClassPers.InsGrowth = 1.75;
                    ClassPers.Equipable1 = "Dague";
                    ClassPers.Equipable2 = "Arme a feu";

                    while (Competences[i].Nom != "Serment d'Hippocrate")
                        i++;

                    CompJoueur[0] = Competences[i];

                    LblCreationFicheSocial.Text = "Lancez 1D100 + 10 pour la carac de Social";
                    break;

                case "Draconiste":
                    ClassPers.PVGrowth = 1.5;
                    ClassPers.MvtBase = "M";
                    ClassPers.Dégâts = "1D8";
                    ClassPers.PreBase = 20;
                    ClassPers.EsqBase = 30;
                    ClassPers.CritBase = 14;
                    ClassPers.VitGrowth = 1.25;
                    ClassPers.ConsGrowth = 1.25;
                    ClassPers.IntGrowth = 1;
                    ClassPers.ChaGrowth = 1;
                    ClassPers.SauGrowth = 2;
                    ClassPers.InsGrowth = 1.5;
                    ClassPers.Equipable1 = "Poutre";
                    ClassPers.Equipable2 = "Bestipierre";

                    while (Competences[i].Nom != "Debut calme")
                        i++;

                    CompJoueur[0] = Competences[i];

                    LblCreationFicheSocial.Text = "Lancez 1D100 - 10 pour la carac de Social";
                    break;

                case "Penseur":
                    ClassPers.PVGrowth = 1;
                    ClassPers.MvtBase = "M";
                    ClassPers.Dégâts = "1D6";
                    ClassPers.PreBase = 30;
                    ClassPers.EsqBase = 70;
                    ClassPers.CritBase = 4;
                    ClassPers.VitGrowth = 1;
                    ClassPers.ConsGrowth = 1.5;
                    ClassPers.IntGrowth = 1.25;
                    ClassPers.ChaGrowth = 2;
                    ClassPers.SauGrowth = 1.5;
                    ClassPers.InsGrowth = 1.25;
                    ClassPers.Equipable1 = "Tome";
                    ClassPers.Equipable2 = "Pugilat";

                    while (Competences[i].Nom != "Reflexion")
                        i++;

                    CompJoueur[0] = Competences[i];

                    LblCreationFicheSocial.Text = "Lancez 1D100 pour la carac de Social";
                    break;

                default:
                    ClassPers.PVGrowth = 1.75;
                    ClassPers.MvtBase = "M";
                    ClassPers.Dégâts = "1D6";
                    ClassPers.PreBase = 50;
                    ClassPers.EsqBase = 50;
                    ClassPers.CritBase = 16;
                    ClassPers.VitGrowth = 1.5;
                    ClassPers.ConsGrowth = 1;
                    ClassPers.IntGrowth = 0.75;
                    ClassPers.ChaGrowth = 1.5;
                    ClassPers.SauGrowth = 0.5;
                    ClassPers.InsGrowth = 1;
                    ClassPers.Equipable1 = "Lance";
                    ClassPers.Equipable2 = "Transcendance";

                    while (Competences[i].Nom != "Pardon")
                        i++;

                    CompJoueur[0] = Competences[i];

                    LblCreationFicheSocial.Text = "Lancez 1D100 - 20 pour la carac de Social";
                    break;
            }

            ClassPers.PVBase = ClassPers.PVGrowth * 10 + BonusPV;
            ClassPers.PreBase += BonusPre;
            ClassPers.EsqBase += BonusEsq;
            ClassPers.CritBase += BonusCrit;
            ClassPers.VitBase = ClassPers.VitGrowth * 10 + BonusVit;
            ClassPers.ConsBase = ClassPers.ConsGrowth * 10 + BonusCons;
            ClassPers.IntBase = ClassPers.IntGrowth * 10 + BonusInt;
            ClassPers.ChaBase = ClassPers.ChaGrowth * 10 + BonusCha;
            ClassPers.SauBase = ClassPers.SauGrowth * 10 + BonusSau;
            ClassPers.InsBase = ClassPers.InsGrowth * 10 + BonusIns;

            ClassPers.Equipable3 = "";
            ClassPers.Equipable4 = "";
            ClassPers.Equipable5 = "";
            ClassPers.Equipable6 = "";

            CompJoueur[1].Nom = "";
            CompJoueur[2].Nom = "";
            CompJoueur[3].Nom = "";
            CompJoueur[4].Nom = "";
            CompJoueur[5].Nom = "";
            CompJoueur[6].Nom = "";
            CompJoueur[7].Nom = "";

            i = 0;

            while (i < 8)
            {
                inventairejoueur[i].Type = "";
                inventairejoueur[i].Nom = "";
                inventairejoueur[i].Rarete = "";
                inventairejoueur[i].Portee = "";
                inventairejoueur[i].BonusType = "";
                inventairejoueur[i].Bonus = 0;
                inventairejoueur[i].EffetType1 = "";
                inventairejoueur[i].EffetDes1 = "";
                inventairejoueur[i].EffetType2 = "";
                inventairejoueur[i].EffetDes2 = "";
                inventairejoueur[i].EffetType3 = "";
                inventairejoueur[i].EffetDes3 = "";
                inventairejoueur[i].Quantité = 0;
                inventairejoueur[i].Prix = 0;
                inventairejoueur[i].EmplOccupe = false;
                i++;
            }

            ClassPers.ExpAct = 0;
            ClassPers.ExpSup = 50;

            ClassPers.PreEquip = 0;
            ClassPers.EsqEquip = 0;
            ClassPers.CritEquip = 0;
            ClassPers.VitEquip = 0;
            ClassPers.ConsEquip = 0;
            ClassPers.IntEquip = 0;
            ClassPers.ChaEquip = 0;
            ClassPers.SauEquip = 0;
            ClassPers.InsEquip = 0;

            ClassPers.MvtBuff = 0;
            ClassPers.PreBuff = 0;
            ClassPers.EsqBuff = 0;
            ClassPers.CritBuff = 0;
            ClassPers.VitBuff = 0;
            ClassPers.ConsBuff = 0;
            ClassPers.IntBuff = 0;
            ClassPers.ChaBuff = 0;
            ClassPers.SauBuff = 0;
            ClassPers.InsBuff = 0;

            ClassPers.PVTotAct = ClassPers.PVBase;
            ClassPers.PVTotMax = ClassPers.PVBase;
            ClassPers.MvtTot = ClassPers.MvtBase;
            ClassPers.PreTot = ClassPers.PreBase;
            ClassPers.EsqTot = ClassPers.EsqBase;
            ClassPers.CritTot = ClassPers.CritBase;
            ClassPers.VitTot = ClassPers.VitBase;
            ClassPers.ConsTot = ClassPers.ConsBase;
            ClassPers.IntTot = ClassPers.IntBase;
            ClassPers.ChaTot = ClassPers.ChaBase;
            ClassPers.SauTot = ClassPers.SauBase;
            ClassPers.InsTot = ClassPers.InsBase;
        }

        //Fonctions compétences
        //Panel compétences

        public void apparitionBoutonsComp()
        {
            if (CompJoueur[0].Nom != "" && CompJoueur[0].Type == "Active")
            {
                BtnComp1.Visible = true;
                BtnComp1.Text = CompJoueur[0].Nom;

                if (CompJoueur[0].CD > 1)
                {
                    LblCompCD1.Text = CompJoueur[0].CD.ToString() + " tours";
                }
                else
                {
                    if (CompJoueur[0].CD == 1)
                    {
                        LblCompCD1.Text = CompJoueur[0].CD.ToString() + " tour";
                    }
                    else
                    {
                        LblCompCD1.Text = "      /";
                    }
                }

                LblCompDes1.Text = CompJoueur[0].Effet;
            }
            else
            {
                BtnComp1.Visible = false;
            }

            if (CompJoueur[1].Nom != "" && CompJoueur[1].Type == "Active")
            {
                BtnComp2.Visible = true;
                BtnComp2.Text = CompJoueur[1].Nom;

                if (CompJoueur[1].CD > 1)
                {
                    LblCompCD2.Text = CompJoueur[1].CD.ToString() + " tours";
                }
                else
                {
                    if (CompJoueur[1].CD == 1)
                    {
                        LblCompCD2.Text = CompJoueur[1].CD.ToString() + " tour";
                    }
                    else
                    {
                        LblCompCD2.Text = "      /";
                    }
                }

                LblCompDes2.Text = CompJoueur[1].Effet;
            }
            else
            {
                BtnComp2.Visible = false;
            }

            if (CompJoueur[2].Nom != "" && CompJoueur[2].Type == "Active")
            {
                BtnComp3.Visible = true;
                BtnComp3.Text = CompJoueur[2].Nom;

                if (CompJoueur[2].CD > 1)
                {
                    LblCompCD3.Text = CompJoueur[2].CD.ToString() + " tours";
                }
                else
                {
                    if (CompJoueur[2].CD == 1)
                    {
                        LblCompCD3.Text = CompJoueur[2].CD.ToString() + " tour";
                    }
                    else
                    {
                        LblCompCD3.Text = "      /";
                    }
                }

                LblCompDes3.Text = CompJoueur[2].Effet;
            }
            else
            {
                BtnComp3.Visible = false;
            }

            if (CompJoueur[3].Nom != "" && CompJoueur[3].Type == "Active")
            {
                BtnComp4.Visible = true;
                BtnComp4.Text = CompJoueur[3].Nom;

                if (CompJoueur[3].CD > 1)
                {
                    LblCompCD4.Text = CompJoueur[3].CD.ToString() + " tours";
                }
                else
                {
                    if (CompJoueur[3].CD == 1)
                    {
                        LblCompCD4.Text = CompJoueur[3].CD.ToString() + " tour";
                    }
                    else
                    {
                        LblCompCD4.Text = "      /";
                    }
                }

                LblCompDes4.Text = CompJoueur[3].Effet;
            }
            else
            {
                BtnComp4.Visible = false;
            }

            if (CompJoueur[4].Nom != "" && CompJoueur[4].Type == "Active")
            {
                BtnComp5.Visible = true;
                BtnComp5.Text = CompJoueur[4].Nom;

                if (CompJoueur[4].CD > 1)
                {
                    LblCompCD5.Text = CompJoueur[4].CD.ToString() + " tours";
                }
                else
                {
                    if (CompJoueur[4].CD == 1)
                    {
                        LblCompCD5.Text = CompJoueur[4].CD.ToString() + " tour";
                    }
                    else
                    {
                        LblCompCD5.Text = "      /";
                    }
                }

                LblCompDes5.Text = CompJoueur[4].Effet;
            }
            else
            {
                BtnComp5.Visible = false;
            }

            if (CompJoueur[5].Nom != "" && CompJoueur[5].Type == "Active")
            {
                BtnComp6.Visible = true;
                BtnComp6.Text = CompJoueur[5].Nom;

                if (CompJoueur[5].CD > 1)
                {
                    LblCompCD6.Text = CompJoueur[5].CD.ToString() + " tours";
                }
                else
                {
                    if (CompJoueur[5].CD == 1)
                    {
                        LblCompCD6.Text = CompJoueur[5].CD.ToString() + " tour";
                    }
                    else
                    {
                        LblCompCD6.Text = "      /";
                    }
                }

                LblCompDes6.Text = CompJoueur[5].Effet;
            }
            else
            {
                BtnComp6.Visible = false;
            }

            if (CompJoueur[6].Nom != "" && CompJoueur[6].Type == "Active")
            {
                BtnComp7.Visible = true;
                BtnComp7.Text = CompJoueur[6].Nom;

                if (CompJoueur[6].CD > 1)
                {
                    LblCompCD7.Text = CompJoueur[6].CD.ToString() + " tours";
                }
                else
                {
                    if (CompJoueur[6].CD == 1)
                    {
                        LblCompCD7.Text = CompJoueur[6].CD.ToString() + " tour";
                    }
                    else
                    {
                        LblCompCD7.Text = "      /";
                    }
                }

                LblCompDes7.Text = CompJoueur[6].Effet;
            }
            else
            {
                BtnComp7.Visible = false;
            }

            if (CompJoueur[7].Nom != "" && CompJoueur[7].Type == "Active")
            {
                BtnComp8.Visible = true;
                BtnComp8.Text = CompJoueur[7].Nom;

                if (CompJoueur[7].CD > 1)
                {
                    LblCompCD8.Text = CompJoueur[7].CD.ToString() + " tours";
                }
                else
                {
                    if (CompJoueur[7].CD == 1)
                    {
                        LblCompCD8.Text = CompJoueur[7].CD.ToString() + " tour";
                    }
                    else
                    {
                        LblCompCD8.Text = "      /";
                    }
                }

                LblCompDes8.Text = CompJoueur[7].Effet;
            }
            else
            {
                BtnComp8.Visible = false;
            }
        }

        //Ad-or-ation

        public void fctCompAd_or_ation(ref Label LblComp)
        {
            LblComp.Text = "Ad-or-ation (Dégâts +" + (int)(Fonds / 500) + ")";
            if ((int)(Fonds / 500) != 0)
                LblComp.ForeColor = Color.Red;
            else
                LblComp.ForeColor = Color.Transparent;
        }

        //Alchimie - Alchimiracle

        public void fctCompAlchimie(ref Label LblComp)
        {
            apparitionBtnCompAlchimieType();

            PnlCompAlchimie1.Location = new Point(0, 0);
            PnlCompAlchimie1.Visible = true;
            PnlComp.Visible = false;
        }

        public void apparitionBtnCompAlchimieType()
        {
            if (InventaireJoueur[0].Type != "Soin" && InventaireJoueur[0].Type != "Degats" && InventaireJoueur[0].Type != "Buff" && InventaireJoueur[0].Type != "Debuff")
            {
                BtnCompAlchimieObj1.Visible = false;
            }
            else
            {
                BtnCompAlchimieObj1.Visible = true;
                BtnCompAlchimieObj1.Text = InventaireJoueur[0].Nom;
            }

            if (InventaireJoueur[1].Type != "Soin" && InventaireJoueur[1].Type != "Degats" && InventaireJoueur[1].Type != "Buff" && InventaireJoueur[1].Type != "Debuff")
            {
                BtnCompAlchimieObj2.Visible = false;
            }
            else
            {
                BtnCompAlchimieObj2.Visible = true;
                BtnCompAlchimieObj2.Text = InventaireJoueur[1].Nom;
            }

            if (InventaireJoueur[2].Type != "Soin" && InventaireJoueur[2].Type != "Degats" && InventaireJoueur[2].Type != "Buff" && InventaireJoueur[2].Type != "Debuff")
            {
                BtnCompAlchimieObj3.Visible = false;
            }
            else
            {
                BtnCompAlchimieObj3.Visible = true;
                BtnCompAlchimieObj3.Text = InventaireJoueur[2].Nom;
            }

            if (InventaireJoueur[3].Type != "Soin" && InventaireJoueur[3].Type != "Degats" && InventaireJoueur[3].Type != "Buff" && InventaireJoueur[3].Type != "Debuff")
            {
                BtnCompAlchimieObj4.Visible = false;
            }
            else
            {
                BtnCompAlchimieObj4.Visible = true;
                BtnCompAlchimieObj4.Text = InventaireJoueur[3].Nom;
            }

            if (InventaireJoueur[4].Type != "Soin" && InventaireJoueur[4].Type != "Degats" && InventaireJoueur[4].Type != "Buff" && InventaireJoueur[4].Type != "Debuff")
            {
                BtnCompAlchimieObj5.Visible = false;
            }
            else
            {
                BtnCompAlchimieObj5.Visible = true;
                BtnCompAlchimieObj5.Text = InventaireJoueur[4].Nom;
            }

            if (InventaireJoueur[5].Type != "Soin" && InventaireJoueur[5].Type != "Degats" && InventaireJoueur[5].Type != "Buff" && InventaireJoueur[5].Type != "Debuff")
            {
                BtnCompAlchimieObj6.Visible = false;
            }
            else
            {
                BtnCompAlchimieObj6.Visible = true;
                BtnCompAlchimieObj6.Text = InventaireJoueur[5].Nom;
            }

            if (InventaireJoueur[6].Type != "Soin" && InventaireJoueur[6].Type != "Degats" && InventaireJoueur[6].Type != "Buff" && InventaireJoueur[6].Type != "Debuff")
            {
                BtnCompAlchimieObj7.Visible = false;
            }
            else
            {
                BtnCompAlchimieObj7.Visible = true;
                BtnCompAlchimieObj7.Text = InventaireJoueur[6].Nom;
            }

            if (InventaireJoueur[7].Type != "Soin" && InventaireJoueur[7].Type != "Degats" && InventaireJoueur[7].Type != "Buff" && InventaireJoueur[7].Type != "Debuff")
            {
                BtnCompAlchimieObj8.Visible = false;
            }
            else
            {
                BtnCompAlchimieObj8.Visible = true;
                BtnCompAlchimieObj8.Text = InventaireJoueur[7].Nom;
            }
        }

        public void apparitionBtnCompAlchimieRarete(ref string rarete)
        {
            if (BtnCompAlchimieObj1.Visible == true && InventaireJoueur[0].Rarete == rarete)
            {
                BtnCompAlchimieObj1.Visible = true;
            }
            else
            {
                BtnCompAlchimieObj1.Visible = false;
            }

            if (BtnCompAlchimieObj2.Visible == true && InventaireJoueur[1].Rarete == rarete)
            {
                BtnCompAlchimieObj2.Visible = true;
            }
            else
            {
                BtnCompAlchimieObj2.Visible = false;
            }

            if (BtnCompAlchimieObj3.Visible == true && InventaireJoueur[2].Rarete == rarete)
            {
                BtnCompAlchimieObj3.Visible = true;
            }
            else
            {
                BtnCompAlchimieObj3.Visible = false;
            }

            if (BtnCompAlchimieObj4.Visible == true && InventaireJoueur[3].Rarete == rarete)
            {
                BtnCompAlchimieObj4.Visible = true;
            }
            else
            {
                BtnCompAlchimieObj4.Visible = false;
            }

            if (BtnCompAlchimieObj5.Visible == true && InventaireJoueur[4].Rarete == rarete)
            {
                BtnCompAlchimieObj5.Visible = true;
            }
            else
            {
                BtnCompAlchimieObj5.Visible = false;
            }

            if (BtnCompAlchimieObj6.Visible == true && InventaireJoueur[5].Rarete == rarete)
            {
                BtnCompAlchimieObj6.Visible = true;
            }
            else
            {
                BtnCompAlchimieObj6.Visible = false;
            }

            if (BtnCompAlchimieObj7.Visible == true && InventaireJoueur[6].Rarete == rarete)
            {
                BtnCompAlchimieObj7.Visible = true;
            }
            else
            {
                BtnCompAlchimieObj7.Visible = false;
            }

            if (BtnCompAlchimieObj8.Visible == true && InventaireJoueur[7].Rarete == rarete)
            {
                BtnCompAlchimieObj8.Visible = true;
            }
            else
            {
                BtnCompAlchimieObj8.Visible = false;
            }
        }

        public void actualisationTxtBoxCompAlchimie()
        {
            TxtBoxCompAlchimieChx1.Text = "Objet 1 : " + comp_alchimie_obj1;
            TxtBoxCompAlchimieChx2.Text = "Objet 2 : " + comp_alchimie_obj2;
        }

        //Custom

        public void fctCompCustom(string lettre)
        {
            switch (lettre)
            {
                case "A": 
                    nb_pts_custom = 3;
                    break;
                case "B":
                    nb_pts_custom = 5;
                    break;
                case "C":
                    nb_pts_custom = 10;
                    break;
                default:
                    nb_pts_custom = 20;
                    break;
            }

            LblCompCustomPtsRestants.Text = "Points à attribuer : " + nb_pts_custom;
            if(Talent == "Bizarre")
            {
                LblCompCustomRequisPV.Text = "Requis : 1";
                LblCompCustomRequisVit.Text = "Requis : 2";
            }
            else
            {
                LblCompCustomRequisPV.Text = "Requis : 2";
                LblCompCustomRequisVit.Text = "Requis : 1";
            }

            ptPV = 0;
            ptPre = 0;
            ptEsq = 0;
            ptCrit = 0;
            ptVit = 0;
            ptCons = 0;
            ptInt = 0;
            ptCha = 0;
            ptSau = 0;
            ptIns = 0;
            nb_pts_custom_apres = nb_pts_custom;

            PnlMarchéAchat.Visible = false;
            PnlCompCustom.Location = new Point(0, 0);
            PnlCompCustom.Visible = true;
        }

        public void attributionPtsCustom(string type_pt, int nb_pt_utilises)
        {
            switch (type_pt)
            {
                case "PV":
                    ptPV++;
                    break;
                case "Pre":
                    ptPre++;
                    break;
                case "Esq":
                    ptEsq++;
                    break;
                case "Crit":
                    ptCrit++;
                    break;
                case "Vit":
                    ptVit++;
                    break;
                case "Cons":
                    ptCons++;
                    break;
                case "Int":
                    ptInt++;
                    break;
                case "Cha":
                    ptCha++;
                    break;
                case "Sau":
                    ptSau++;
                    break;
                default:
                    ptIns++;
                    break;
            }

            nb_pts_custom_apres -= nb_pt_utilises;

            TxtBoxCompCustom.Text = "";

            if (ptPV != 0)
            {
                TxtBoxCompCustom.Text += "PV +" + ptPV.ToString();
            }

            if (ptPre != 0 && ptPV != 0)
            {
                TxtBoxCompCustom.Text += "; Pré +" + ptPre.ToString();
            }
            else
            {
                if (ptPre != 0 && ptPV == 0)
                {
                    TxtBoxCompCustom.Text += "Pré +" + ptPre.ToString();
                }
            }

            if (ptEsq != 0 && (ptPV != 0 || ptPre != 0))
            {
                TxtBoxCompCustom.Text += "; Esq +" + ptEsq.ToString();
            }
            else
            {
                if (ptEsq != 0 && ptPV == 0 && ptPre == 0)
                {
                    TxtBoxCompCustom.Text += "Esq +" + ptEsq.ToString();
                }
            }

            if (ptCrit != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0))
            {
                TxtBoxCompCustom.Text += "; Crit +" + ptCrit.ToString();
            }
            else
            {
                if (ptCrit != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0)
                {
                    TxtBoxCompCustom.Text += "Crit +" + ptCrit.ToString();
                }
            }

            if (ptVit != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0))
            {
                TxtBoxCompCustom.Text += "; Vit +" + ptVit.ToString();
            }
            else
            {
                if (ptVit != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0)
                {
                    TxtBoxCompCustom.Text += "Vit +" + ptVit.ToString();
                }
            }

            if (ptCons != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0 || ptVit != 0))
            {
                TxtBoxCompCustom.Text += "; Cons +" + ptCons.ToString();
            }
            else
            {
                if (ptCons != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0 && ptVit == 0)
                {
                    TxtBoxCompCustom.Text += "Cons +" + ptCons.ToString();
                }
            }

            if (ptInt != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0 || ptVit != 0 || ptCons != 0))
            {
                TxtBoxCompCustom.Text += "; Int +" + ptInt.ToString();
            }
            else
            {
                if (ptInt != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0 && ptVit == 0 && ptCons == 0)
                {
                    TxtBoxCompCustom.Text += "Int +" + ptInt.ToString();
                }
            }

            if (ptCha != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0 || ptVit != 0 || ptCons != 0 || ptInt != 0))
            {
                TxtBoxCompCustom.Text += "; Cha +" + ptCha.ToString();
            }
            else
            {
                if (ptCha != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0 && ptVit == 0 && ptCons == 0 && ptInt == 0)
                {
                    TxtBoxCompCustom.Text += "Cha +" + ptCha.ToString();
                }
            }

            if (ptSau != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0 || ptVit != 0 || ptCons != 0 || ptInt != 0 || ptCha != 0))
            {
                TxtBoxCompCustom.Text += "; Sau +" + ptSau.ToString();
            }
            else
            {
                if (ptSau != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0 && ptVit == 0 && ptCons == 0 && ptInt == 0 && ptCha == 0)
                {
                    TxtBoxCompCustom.Text += "Sau +" + ptSau.ToString();
                }
            }

            if (ptIns != 0 && (ptPV != 0 || ptPre != 0 || ptEsq != 0 || ptCrit != 0 || ptVit != 0 || ptCons != 0 || ptInt != 0 || ptCha != 0 || ptSau != 0))
            {
                TxtBoxCompCustom.Text += "; Ins +" + ptIns.ToString();
            }
            else
            {
                if (ptIns != 0 && ptPV == 0 && ptPre == 0 && ptEsq == 0 && ptCrit == 0 && ptVit == 0 && ptCons == 0 && ptInt == 0 && ptCha == 0 && ptSau == 0)
                {
                    TxtBoxCompCustom.Text += "Ins +" + ptIns.ToString();
                }
            }
        }

        //Fonction Sauvegarde Fiche Perso

        public void SauvegardeFichePerso()
        {
            Doc_fiche_perso_ecriture = new FileStream("fiche_perso_stats.txt", FileMode.Open, FileAccess.Write, FileShare.ReadWrite);

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(Chapitre.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, Chapitre.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(Phase);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, Phase.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(Date.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, Date.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(MomentJournée);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, MomentJournée.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.Dégâts);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.Dégâts.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(Fonds.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, Fonds.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ExpAct.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ExpAct.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ExpSup.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ExpSup.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(PtsComp.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, PtsComp.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.NomClasse);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.NomClasse.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(Talent);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, Talent.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(Statut);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, Statut.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(Age);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, Age.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.Equipable1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.Equipable1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.Equipable2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.Equipable2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.Equipable3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.Equipable3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.Equipable4);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.Equipable4.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.Equipable5);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.Equipable5.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.Equipable6);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.Equipable6.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(CompJoueur[0].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, CompJoueur[0].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(CompJoueur[1].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, CompJoueur[1].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(CompJoueur[2].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, CompJoueur[2].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(CompJoueur[3].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, CompJoueur[3].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(CompJoueur[4].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, CompJoueur[4].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(CompJoueur[5].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, CompJoueur[5].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(CompJoueur[6].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, CompJoueur[6].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(CompJoueur[7].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, CompJoueur[7].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.PVBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.PVBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.MvtBase);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.MvtBase.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.SocialBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.SocialBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.PreBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.PreBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.EsqBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.EsqBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.CritBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.CritBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.VitBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.VitBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ConsBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ConsBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.IntBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.IntBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ChaBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ChaBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.SauBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.SauBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.InsBase.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.InsBase.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.PVGrowth.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.PVGrowth.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.VitGrowth.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.VitGrowth.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ConsGrowth.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ConsGrowth.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.IntGrowth.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.IntGrowth.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ChaGrowth.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ChaGrowth.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.SauGrowth.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.SauGrowth.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.InsGrowth.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.InsGrowth.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.PreEquip.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.PreEquip.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.EsqEquip.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.EsqEquip.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.CritEquip.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.CritEquip.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.VitEquip.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.VitEquip.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ConsEquip.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ConsEquip.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.IntEquip.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.IntEquip.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ChaEquip.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ChaEquip.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.SauEquip.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.SauEquip.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.InsEquip.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.InsEquip.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.MvtBuff.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.MvtBuff.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.PreBuff.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.PreBuff.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.EsqBuff.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.EsqBuff.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.CritBuff.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.CritBuff.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.VitBuff.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.VitBuff.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ConsBuff.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ConsBuff.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.IntBuff.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.IntBuff.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ChaBuff.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ChaBuff.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.SauBuff.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.SauBuff.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.InsBuff.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.InsBuff.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.PVTotAct.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.PVTotAct.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.PVTotMax.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.PVTotMax.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.MvtTot);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.MvtTot.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.SocialTot.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.SocialTot.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.PreTot.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.PreTot.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.EsqTot.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.EsqTot.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.CritTot.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.CritTot.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.VitTot.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.VitTot.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ConsTot.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ConsTot.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.IntTot.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.IntTot.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.ChaTot.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.ChaTot.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.SauTot.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.SauTot.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.InsTot.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.InsTot.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].Type);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].Type.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].Rarete);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].Rarete.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].BonusType);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].BonusType.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].Bonus.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].Bonus.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].Portee);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].Portee.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].EffetType1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].EffetType1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].EffetDes1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].EffetDes1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].EffetType2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].EffetType2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].EffetDes2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].EffetDes2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].EffetType3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].EffetType3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].EffetDes3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].EffetDes3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].Prix.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].Prix.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].Quantité.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].Quantité.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[0].EmplOccupe.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[0].EmplOccupe.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].Type);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].Type.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].Rarete);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].Rarete.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].BonusType);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].BonusType.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].Bonus.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].Bonus.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].Portee);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].Portee.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].EffetType1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].EffetType1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].EffetDes1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].EffetDes1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].EffetType2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].EffetType2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].EffetDes2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].EffetDes2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].EffetType3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].EffetType3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].EffetDes3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].EffetDes3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].Prix.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].Prix.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].Quantité.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].Quantité.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[1].EmplOccupe.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[1].EmplOccupe.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].Type);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].Type.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].Rarete);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].Rarete.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].BonusType);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].BonusType.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].Bonus.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].Bonus.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].Portee);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].Portee.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].EffetType1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].EffetType1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].EffetDes1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].EffetDes1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].EffetType2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].EffetType2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].EffetDes2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].EffetDes2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].EffetType3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].EffetType3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].EffetDes3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].EffetDes3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].Prix.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].Prix.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].Quantité.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].Quantité.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[2].EmplOccupe.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[2].EmplOccupe.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].Type);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].Type.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].Rarete);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].Rarete.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].BonusType);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].BonusType.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].Bonus.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].Bonus.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].Portee);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].Portee.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].EffetType1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].EffetType1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].EffetDes1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].EffetDes1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].EffetType2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].EffetType2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].EffetDes2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].EffetDes2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].EffetType3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].EffetType3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].EffetDes3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].EffetDes3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].Prix.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].Prix.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].Quantité.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].Quantité.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[3].EmplOccupe.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[3].EmplOccupe.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].Type);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].Type.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].Rarete);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].Rarete.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].BonusType);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].BonusType.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].Bonus.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].Bonus.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].Portee);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].Portee.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].EffetType1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].EffetType1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].EffetDes1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].EffetDes1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].EffetType2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].EffetType2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].EffetDes2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].EffetDes2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].EffetType3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].EffetType3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].EffetDes3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].EffetDes3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].Prix.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].Prix.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].Quantité.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].Quantité.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[4].EmplOccupe.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[4].EmplOccupe.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].Type);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].Type.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].Rarete);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].Rarete.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].BonusType);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].BonusType.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].Bonus.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].Bonus.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].Portee);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].Portee.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].EffetType1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].EffetType1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].EffetDes1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].EffetDes1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].EffetType2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].EffetType2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].EffetDes2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].EffetDes2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].EffetType3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].EffetType3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].EffetDes3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].EffetDes3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].Prix.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].Prix.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].Quantité.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].Quantité.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[5].EmplOccupe.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[5].EmplOccupe.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].Type);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].Type.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].Rarete);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].Rarete.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].BonusType);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].BonusType.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].Bonus.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].Bonus.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].Portee);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].Portee.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].EffetType1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].EffetType1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].EffetDes1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].EffetDes1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].EffetType2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].EffetType2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].EffetDes2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].EffetDes2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].EffetType3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].EffetType3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].EffetDes3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].EffetDes3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].Prix.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].Prix.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].Quantité.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].Quantité.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[6].EmplOccupe.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[6].EmplOccupe.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].Nom);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].Nom.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].Type);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].Type.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].Rarete);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].Rarete.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].BonusType);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].BonusType.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].Bonus.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].Bonus.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].Portee);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].Portee.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].EffetType1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].EffetType1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].EffetDes1);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].EffetDes1.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].EffetType2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].EffetType2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].EffetDes2);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].EffetDes2.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].EffetType3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].EffetType3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].EffetDes3);
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].EffetDes3.Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].Prix.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].Prix.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].Quantité.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].Quantité.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(InventaireJoueur[7].EmplOccupe.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, InventaireJoueur[7].EmplOccupe.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(ClassePerso.Niv.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, ClassePerso.Niv.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Ecriture_fiche_perso = Encoding.UTF8.GetBytes(tour_combat.ToString());
            Doc_fiche_perso_ecriture.Write(Ecriture_fiche_perso, 0, tour_combat.ToString().Length);
            Doc_fiche_perso_ecriture.WriteByte(Convert.ToByte('\n'));

            Doc_fiche_perso_ecriture.Close();
        }

        //Fonction Charger Fiche Perso

        public void ChargerFichePerso()
        {
            Doc_fiche_perso_ecriture = new FileStream("fiche_perso_stats.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            LignesFichierSauvegarde = File.ReadAllLines("fiche_perso_stats.txt");

            Doc_fiche_perso_ecriture.Close();

            Chapitre = Convert.ToInt32(LignesFichierSauvegarde[0]);
            Phase = LignesFichierSauvegarde[1];
            Date = Convert.ToInt32(LignesFichierSauvegarde[2]);
            MomentJournée = LignesFichierSauvegarde[3];

            Nom = LignesFichierSauvegarde[4];
            ClassePerso.Dégâts = LignesFichierSauvegarde[5];

            Fonds = Convert.ToInt32(LignesFichierSauvegarde[6]);
            ClassePerso.ExpAct = Convert.ToInt32(LignesFichierSauvegarde[7]);
            ClassePerso.ExpSup = Convert.ToInt32(LignesFichierSauvegarde[8]);
            PtsComp = Convert.ToInt32(LignesFichierSauvegarde[9]);

            ClassePerso.NomClasse = LignesFichierSauvegarde[10];
            Talent = LignesFichierSauvegarde[11];
            Statut = LignesFichierSauvegarde[12];
            Age = LignesFichierSauvegarde[13];

            ClassePerso.Equipable1 = LignesFichierSauvegarde[14];
            ClassePerso.Equipable2 = LignesFichierSauvegarde[15];
            ClassePerso.Equipable3 = LignesFichierSauvegarde[16];
            ClassePerso.Equipable4 = LignesFichierSauvegarde[17];
            ClassePerso.Equipable5 = LignesFichierSauvegarde[18];
            ClassePerso.Equipable6 = LignesFichierSauvegarde[19];

            CompJoueur[0].Nom = LignesFichierSauvegarde[20];
            CompJoueur[1].Nom = LignesFichierSauvegarde[21];
            CompJoueur[2].Nom = LignesFichierSauvegarde[22];
            CompJoueur[3].Nom = LignesFichierSauvegarde[23];
            CompJoueur[4].Nom = LignesFichierSauvegarde[24];
            CompJoueur[5].Nom = LignesFichierSauvegarde[25];
            CompJoueur[6].Nom = LignesFichierSauvegarde[26];
            CompJoueur[7].Nom = LignesFichierSauvegarde[27];

            ClassePerso.PVBase = (int)Convert.ToDouble(LignesFichierSauvegarde[28]);
            ClassePerso.MvtBase = LignesFichierSauvegarde[29];
            ClassePerso.SocialBase = Convert.ToInt32(LignesFichierSauvegarde[30]);
            ClassePerso.PreBase = Convert.ToInt32(LignesFichierSauvegarde[31]);
            ClassePerso.EsqBase = Convert.ToInt32(LignesFichierSauvegarde[32]);
            ClassePerso.CritBase = Convert.ToInt32(LignesFichierSauvegarde[33]);
            ClassePerso.VitBase = (int)Convert.ToDouble(LignesFichierSauvegarde[34]);
            ClassePerso.ConsBase = (int)Convert.ToDouble(LignesFichierSauvegarde[35]);
            ClassePerso.IntBase = (int)Convert.ToDouble(LignesFichierSauvegarde[36]);
            ClassePerso.ChaBase = (int)Convert.ToDouble(LignesFichierSauvegarde[37]);
            ClassePerso.SauBase = (int)Convert.ToDouble(LignesFichierSauvegarde[38]);
            ClassePerso.InsBase = (int)Convert.ToDouble(LignesFichierSauvegarde[39]);

            ClassePerso.PVGrowth = Convert.ToDouble(LignesFichierSauvegarde[40]);
            ClassePerso.VitGrowth = Convert.ToDouble(LignesFichierSauvegarde[41]);
            ClassePerso.ConsGrowth = Convert.ToDouble(LignesFichierSauvegarde[42]);
            ClassePerso.IntGrowth = Convert.ToDouble(LignesFichierSauvegarde[43]);
            ClassePerso.ChaGrowth = Convert.ToDouble(LignesFichierSauvegarde[44]);
            ClassePerso.SauGrowth = Convert.ToDouble(LignesFichierSauvegarde[45]);
            ClassePerso.InsGrowth = Convert.ToDouble(LignesFichierSauvegarde[46]);

            ClassePerso.PreEquip = Convert.ToInt32(LignesFichierSauvegarde[47]);
            ClassePerso.EsqEquip = Convert.ToInt32(LignesFichierSauvegarde[48]);
            ClassePerso.CritEquip = Convert.ToInt32(LignesFichierSauvegarde[49]);
            ClassePerso.VitEquip = Convert.ToInt32(LignesFichierSauvegarde[50]);
            ClassePerso.ConsEquip = Convert.ToInt32(LignesFichierSauvegarde[51]);
            ClassePerso.IntEquip = Convert.ToInt32(LignesFichierSauvegarde[52]);
            ClassePerso.ChaEquip = Convert.ToInt32(LignesFichierSauvegarde[53]);
            ClassePerso.SauEquip = Convert.ToInt32(LignesFichierSauvegarde[54]);
            ClassePerso.InsEquip = Convert.ToInt32(LignesFichierSauvegarde[55]);

            ClassePerso.MvtBuff = Convert.ToInt32(LignesFichierSauvegarde[56]);
            ClassePerso.PreBuff = Convert.ToInt32(LignesFichierSauvegarde[57]);
            ClassePerso.EsqBuff = Convert.ToInt32(LignesFichierSauvegarde[58]);
            ClassePerso.CritBuff = Convert.ToInt32(LignesFichierSauvegarde[59]);
            ClassePerso.VitBuff = Convert.ToInt32(LignesFichierSauvegarde[60]);
            ClassePerso.ConsBuff = Convert.ToInt32(LignesFichierSauvegarde[61]);
            ClassePerso.IntBuff = Convert.ToInt32(LignesFichierSauvegarde[62]);
            ClassePerso.ChaBuff = Convert.ToInt32(LignesFichierSauvegarde[63]);
            ClassePerso.SauBuff = Convert.ToInt32(LignesFichierSauvegarde[64]);
            ClassePerso.InsBuff = Convert.ToInt32(LignesFichierSauvegarde[65]);

            ClassePerso.PVTotAct = (int)Convert.ToDouble(LignesFichierSauvegarde[66]);
            ClassePerso.PVTotMax = (int)Convert.ToDouble(LignesFichierSauvegarde[67]);
            ClassePerso.MvtTot = LignesFichierSauvegarde[68];
            ClassePerso.SocialTot = Convert.ToInt32(LignesFichierSauvegarde[69]);
            ClassePerso.PreTot = Convert.ToInt32(LignesFichierSauvegarde[70]);
            ClassePerso.EsqTot = Convert.ToInt32(LignesFichierSauvegarde[71]);
            ClassePerso.CritTot = Convert.ToInt32(LignesFichierSauvegarde[72]);
            ClassePerso.VitTot = (int)Convert.ToDouble(LignesFichierSauvegarde[73]);
            ClassePerso.ConsTot = (int)Convert.ToDouble(LignesFichierSauvegarde[74]);
            ClassePerso.IntTot = (int)Convert.ToDouble(LignesFichierSauvegarde[75]);
            ClassePerso.ChaTot = (int)Convert.ToDouble(LignesFichierSauvegarde[76]);
            ClassePerso.SauTot = (int)Convert.ToDouble(LignesFichierSauvegarde[77]);
            ClassePerso.InsTot = (int)Convert.ToDouble(LignesFichierSauvegarde[78]);

            InventaireJoueur[0].Nom = LignesFichierSauvegarde[79];
            InventaireJoueur[0].Type = LignesFichierSauvegarde[80];
            InventaireJoueur[0].Rarete = LignesFichierSauvegarde[81];
            InventaireJoueur[0].BonusType = LignesFichierSauvegarde[82];
            InventaireJoueur[0].Bonus = Convert.ToInt32(LignesFichierSauvegarde[83]);
            InventaireJoueur[0].Portee = LignesFichierSauvegarde[84];
            InventaireJoueur[0].EffetType1 = LignesFichierSauvegarde[85];
            InventaireJoueur[0].EffetDes1 = LignesFichierSauvegarde[86];
            InventaireJoueur[0].EffetType2 = LignesFichierSauvegarde[87];
            InventaireJoueur[0].EffetDes2 = LignesFichierSauvegarde[88];
            InventaireJoueur[0].EffetType3 = LignesFichierSauvegarde[89];
            InventaireJoueur[0].EffetDes3 = LignesFichierSauvegarde[90];
            InventaireJoueur[0].Prix = Convert.ToInt32(LignesFichierSauvegarde[91]);
            InventaireJoueur[0].Quantité = Convert.ToInt32(LignesFichierSauvegarde[92]);
            InventaireJoueur[0].EmplOccupe = Convert.ToBoolean(LignesFichierSauvegarde[93]);

            InventaireJoueur[1].Nom = LignesFichierSauvegarde[94];
            InventaireJoueur[1].Type = LignesFichierSauvegarde[95];
            InventaireJoueur[1].Rarete = LignesFichierSauvegarde[96];
            InventaireJoueur[1].BonusType = LignesFichierSauvegarde[97];
            InventaireJoueur[1].Bonus = Convert.ToInt32(LignesFichierSauvegarde[98]);
            InventaireJoueur[1].Portee = LignesFichierSauvegarde[99];
            InventaireJoueur[1].EffetType1 = LignesFichierSauvegarde[100];
            InventaireJoueur[1].EffetDes1 = LignesFichierSauvegarde[101];
            InventaireJoueur[1].EffetType2 = LignesFichierSauvegarde[102];
            InventaireJoueur[1].EffetDes2 = LignesFichierSauvegarde[103];
            InventaireJoueur[1].EffetType3 = LignesFichierSauvegarde[104];
            InventaireJoueur[1].EffetDes3 = LignesFichierSauvegarde[105];
            InventaireJoueur[1].Prix = Convert.ToInt32(LignesFichierSauvegarde[106]);
            InventaireJoueur[1].Quantité = Convert.ToInt32(LignesFichierSauvegarde[107]);
            InventaireJoueur[1].EmplOccupe = Convert.ToBoolean(LignesFichierSauvegarde[108]);

            InventaireJoueur[2].Nom = LignesFichierSauvegarde[109];
            InventaireJoueur[2].Type = LignesFichierSauvegarde[110];
            InventaireJoueur[2].Rarete = LignesFichierSauvegarde[111];
            InventaireJoueur[2].BonusType = LignesFichierSauvegarde[112];
            InventaireJoueur[2].Bonus = Convert.ToInt32(LignesFichierSauvegarde[113]);
            InventaireJoueur[2].Portee = LignesFichierSauvegarde[114];
            InventaireJoueur[2].EffetType1 = LignesFichierSauvegarde[115];
            InventaireJoueur[2].EffetDes1 = LignesFichierSauvegarde[116];
            InventaireJoueur[2].EffetType2 = LignesFichierSauvegarde[117];
            InventaireJoueur[2].EffetDes2 = LignesFichierSauvegarde[118];
            InventaireJoueur[2].EffetType3 = LignesFichierSauvegarde[119];
            InventaireJoueur[2].EffetDes3 = LignesFichierSauvegarde[120];
            InventaireJoueur[2].Prix = Convert.ToInt32(LignesFichierSauvegarde[121]);
            InventaireJoueur[2].Quantité = Convert.ToInt32(LignesFichierSauvegarde[122]);
            InventaireJoueur[2].EmplOccupe = Convert.ToBoolean(LignesFichierSauvegarde[123]);

            InventaireJoueur[3].Nom = LignesFichierSauvegarde[124];
            InventaireJoueur[3].Type = LignesFichierSauvegarde[125];
            InventaireJoueur[3].Rarete = LignesFichierSauvegarde[126];
            InventaireJoueur[3].BonusType = LignesFichierSauvegarde[127];
            InventaireJoueur[3].Bonus = Convert.ToInt32(LignesFichierSauvegarde[128]);
            InventaireJoueur[3].Portee = LignesFichierSauvegarde[129];
            InventaireJoueur[3].EffetType1 = LignesFichierSauvegarde[130];
            InventaireJoueur[3].EffetDes1 = LignesFichierSauvegarde[131];
            InventaireJoueur[3].EffetType2 = LignesFichierSauvegarde[132];
            InventaireJoueur[3].EffetDes2 = LignesFichierSauvegarde[133];
            InventaireJoueur[3].EffetType3 = LignesFichierSauvegarde[134];
            InventaireJoueur[3].EffetDes3 = LignesFichierSauvegarde[135];
            InventaireJoueur[3].Prix = Convert.ToInt32(LignesFichierSauvegarde[136]);
            InventaireJoueur[3].Quantité = Convert.ToInt32(LignesFichierSauvegarde[137]);
            InventaireJoueur[3].EmplOccupe = Convert.ToBoolean(LignesFichierSauvegarde[138]);

            InventaireJoueur[4].Nom = LignesFichierSauvegarde[139];
            InventaireJoueur[4].Type = LignesFichierSauvegarde[140];
            InventaireJoueur[4].Rarete = LignesFichierSauvegarde[141];
            InventaireJoueur[4].BonusType = LignesFichierSauvegarde[142];
            InventaireJoueur[4].Bonus = Convert.ToInt32(LignesFichierSauvegarde[143]);
            InventaireJoueur[4].Portee = LignesFichierSauvegarde[144];
            InventaireJoueur[4].EffetType1 = LignesFichierSauvegarde[145];
            InventaireJoueur[4].EffetDes1 = LignesFichierSauvegarde[146];
            InventaireJoueur[4].EffetType2 = LignesFichierSauvegarde[147];
            InventaireJoueur[4].EffetDes2 = LignesFichierSauvegarde[148];
            InventaireJoueur[4].EffetType3 = LignesFichierSauvegarde[149];
            InventaireJoueur[4].EffetDes3 = LignesFichierSauvegarde[150];
            InventaireJoueur[4].Prix = Convert.ToInt32(LignesFichierSauvegarde[151]);
            InventaireJoueur[4].Quantité = Convert.ToInt32(LignesFichierSauvegarde[152]);
            InventaireJoueur[4].EmplOccupe = Convert.ToBoolean(LignesFichierSauvegarde[153]);

            InventaireJoueur[5].Nom = LignesFichierSauvegarde[154];
            InventaireJoueur[5].Type = LignesFichierSauvegarde[155];
            InventaireJoueur[5].Rarete = LignesFichierSauvegarde[156];
            InventaireJoueur[5].BonusType = LignesFichierSauvegarde[157];
            InventaireJoueur[5].Bonus = Convert.ToInt32(LignesFichierSauvegarde[158]);
            InventaireJoueur[5].Portee = LignesFichierSauvegarde[159];
            InventaireJoueur[5].EffetType1 = LignesFichierSauvegarde[160];
            InventaireJoueur[5].EffetDes1 = LignesFichierSauvegarde[161];
            InventaireJoueur[5].EffetType2 = LignesFichierSauvegarde[162];
            InventaireJoueur[5].EffetDes2 = LignesFichierSauvegarde[163];
            InventaireJoueur[5].EffetType3 = LignesFichierSauvegarde[164];
            InventaireJoueur[5].EffetDes3 = LignesFichierSauvegarde[165];
            InventaireJoueur[5].Prix = Convert.ToInt32(LignesFichierSauvegarde[166]);
            InventaireJoueur[5].Quantité = Convert.ToInt32(LignesFichierSauvegarde[167]);
            InventaireJoueur[5].EmplOccupe = Convert.ToBoolean(LignesFichierSauvegarde[168]);

            InventaireJoueur[6].Nom = LignesFichierSauvegarde[169];
            InventaireJoueur[6].Type = LignesFichierSauvegarde[170];
            InventaireJoueur[6].Rarete = LignesFichierSauvegarde[171];
            InventaireJoueur[6].BonusType = LignesFichierSauvegarde[172];
            InventaireJoueur[6].Bonus = Convert.ToInt32(LignesFichierSauvegarde[173]);
            InventaireJoueur[6].Portee = LignesFichierSauvegarde[174];
            InventaireJoueur[6].EffetType1 = LignesFichierSauvegarde[175];
            InventaireJoueur[6].EffetDes1 = LignesFichierSauvegarde[176];
            InventaireJoueur[6].EffetType2 = LignesFichierSauvegarde[177];
            InventaireJoueur[6].EffetDes2 = LignesFichierSauvegarde[178];
            InventaireJoueur[6].EffetType3 = LignesFichierSauvegarde[179];
            InventaireJoueur[6].EffetDes3 = LignesFichierSauvegarde[180];
            InventaireJoueur[6].Prix = Convert.ToInt32(LignesFichierSauvegarde[181]);
            InventaireJoueur[6].Quantité = Convert.ToInt32(LignesFichierSauvegarde[182]);
            InventaireJoueur[6].EmplOccupe = Convert.ToBoolean(LignesFichierSauvegarde[183]);

            InventaireJoueur[7].Nom = LignesFichierSauvegarde[184];
            InventaireJoueur[7].Type = LignesFichierSauvegarde[185];
            InventaireJoueur[7].Rarete = LignesFichierSauvegarde[186];
            InventaireJoueur[7].BonusType = LignesFichierSauvegarde[187];
            InventaireJoueur[7].Bonus = Convert.ToInt32(LignesFichierSauvegarde[188]);
            InventaireJoueur[7].Portee = LignesFichierSauvegarde[189];
            InventaireJoueur[7].EffetType1 = LignesFichierSauvegarde[190];
            InventaireJoueur[7].EffetDes1 = LignesFichierSauvegarde[191];
            InventaireJoueur[7].EffetType2 = LignesFichierSauvegarde[192];
            InventaireJoueur[7].EffetDes2 = LignesFichierSauvegarde[193];
            InventaireJoueur[7].EffetType3 = LignesFichierSauvegarde[194];
            InventaireJoueur[7].EffetDes3 = LignesFichierSauvegarde[195];
            InventaireJoueur[7].Prix = Convert.ToInt32(LignesFichierSauvegarde[196]);
            InventaireJoueur[7].Quantité = Convert.ToInt32(LignesFichierSauvegarde[197]);
            InventaireJoueur[7].EmplOccupe = Convert.ToBoolean(LignesFichierSauvegarde[198]);

            ClassePerso.Niv = Convert.ToInt32(LignesFichierSauvegarde[199]);
            tour_combat = Convert.ToInt32(LignesFichierSauvegarde[200]);

            int i = 0;

            if(CompJoueur[0].Nom != "")
            {
                while (Competences[i].Nom != CompJoueur[0].Nom)
                    i++;

                CompJoueur[0] = Competences[i];
            }

            i = 0;

            if (CompJoueur[1].Nom != "")
            {
                while (Competences[i].Nom != CompJoueur[1].Nom)
                    i++;

                CompJoueur[1] = Competences[i];
            }

            i = 0;

            if (CompJoueur[2].Nom != "")
            {
                while (Competences[i].Nom != CompJoueur[2].Nom)
                    i++;

                CompJoueur[2] = Competences[i];
            }

            i = 0;

            if (CompJoueur[3].Nom != "")
            {
                while (Competences[i].Nom != CompJoueur[3].Nom)
                    i++;

                CompJoueur[3] = Competences[i];
            }

            i = 0;

            if (CompJoueur[4].Nom != "")
            {
                while (Competences[i].Nom != CompJoueur[4].Nom)
                    i++;

                CompJoueur[4] = Competences[i];
            }

            i = 0;

            if (CompJoueur[5].Nom != "")
            {
                while (Competences[i].Nom != CompJoueur[5].Nom)
                    i++;

                CompJoueur[5] = Competences[i];
            }

            i = 0;

            if (CompJoueur[6].Nom != "")
            {
                while (Competences[i].Nom != CompJoueur[6].Nom)
                    i++;

                CompJoueur[6] = Competences[i];
            }

            i = 0;

            if (CompJoueur[7].Nom != "")
            {
                while (Competences[i].Nom != CompJoueur[7].Nom)
                    i++;

                CompJoueur[7] = Competences[i];
            }
        }

        

        //Fonction initialisation inventaire
        public void initInventaire(ref ObjetInv[] InvACompl)
        {
            int i = 0;

            //Epée

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Chevaliere";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Baguette P";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Bouchee";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Slash";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Wakizashi";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Excalidur";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Sanguinolente";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Ponction";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Angurva";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Ruisseau de l'angoisse";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Damocles";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Canon de verre";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Galaad";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Endurcissement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Compagnon";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Al-Battar";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Saucisson";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Repas";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Ardente";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Souffle de feu";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Glaive";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Carmina";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Flamboiement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Kaïbur";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Devier";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Ascalon";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Lame 0";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Letale";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Zanmato";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Fraternite";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Freyr";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Freyr";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Ultima";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Degats extra";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Epee";
            InvACompl[i].Nom = "Excaliburne";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 100;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Defloration";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Lances

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Frene";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Freinage";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Pique";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Epieu";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Fabrication";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Brochette";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Bouchee";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Fourchette";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Casse-dalle";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Double-lame";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Demi-tour";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Tombeciel";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Peur gauloise";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Belier";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Paratonnerre";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Court-circuit";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Unident";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Cùchulainn";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Multidextre";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "PeP";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Porte et Pique";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Dentduo";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Demi-tour";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Penetrante";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Pic a glace";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Gel";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Ameno";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Demi-soin";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Trident";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Satalacgmite";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Nirvana";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 14;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Degats extra";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Longinus";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Navigation";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Trinite";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Activation";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Gungnir";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Verrouillage";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lance";
            InvACompl[i].Nom = "Doigt Deus";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 100;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Designation";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Dagues

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Couteau";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Bouchee";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Papier";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Saignement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Ciseau";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Recoudre";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Vilaine dent";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Cuisse de poulet";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Casse-dalle";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Canif";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Multiusage";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Cloche de service";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Poignard bonifiant";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Scalpel";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Croc carie";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Le feu par le feu";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Depeceur";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Hache-viande";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Couteau a fromage";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Repas";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Napperon dangereux";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Organix";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Transplantation";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Couteau de secours";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Bowie";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Demi-tour";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Ceramique";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Efficacite";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Main coreenne";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Surin";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Ciselee";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Dentgereuse";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Zanmato";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Bouteille de biere";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Fracas";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Dague";
            InvACompl[i].Nom = "Ma bite et mon couteau";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 20;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Assassinat";
            InvACompl[i].EffetType2 = "Objet";
            InvACompl[i].EffetDes2 = "Badass";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Outils

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Filet de peche";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Bouchee";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Fourche";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Ruee";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Marteau";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Assomer";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Baton";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Filet de gladiateur";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Gene";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Faucille";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Scie";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Couperet";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Baguette M";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Fouet";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Correction";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Chaines";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Cle a molette";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Rouleau patissier";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Hachoir a viande";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Hache-viande";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Martinet";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Correction";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Masse";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Enfoncer";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Montre a gousset";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Le Monde";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Matraque";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Correction";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Canne soviet";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Aura d'expert";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Talkie-walkie";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Masse martelee";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Seisme";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Lego";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Fourberie";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Outil";
            InvACompl[i].Nom = "Omnilunettes";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 100;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Reglages";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Transcendance

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Valefore";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Sonic Wing";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Mog";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Makina";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Chocobo";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Ruee";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Ifrit";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Daigoro";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Actarus";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Shiva";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Pampa";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Epines";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Poupée";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Ixion";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Deus ex Machina";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Kelsus";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Levitation";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Tomberry";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Rancune";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Force";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Controle";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Deus Vult";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Jerusalem";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Croix";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Heal";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Méca";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Der Richter";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Vit";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Bahamut";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Aura";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Anima";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Lamento";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Etre Supreme";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Toute-Puissance";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Transcendance";
            InvACompl[i].Nom = "Goldorak";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 20;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Divinisme";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Arcs

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Arc de cercle";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Arc de chene";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Enchenement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Parc";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Arcupidon";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Attirance";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Arctique";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Arquebuse";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Repas";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Art colore";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Archetype";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Archive";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Arche";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Arc-en-ciel";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Archange";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Arcane";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Farce";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Marche";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Marchandage";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Architecture";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Arc de triomphe";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Triomphe";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Art qualitatif";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Embarquement";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Embarquement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Art qualitatif";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Archi fort";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Puissance";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Dark";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Obscurite";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arc";
            InvACompl[i].Nom = "Marc";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "M - L";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 20;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Definition";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Tomes

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Combat rapproche";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Rengorgement";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Calme";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "En garde !";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Rigidite";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Fragilite";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Livre d'astuces";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Tilt";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "I.M.F";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Talisman";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Chance";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Gacha";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Edgar Allan Poe";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Recueil de poemes";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Debuff";
            InvACompl[i].EffetDes1 = "Degout";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Reflexion";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Lightning";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Rapidite";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Encyclopedie";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Pifometre";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Pifometre";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Guide vert";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Promenade";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Bouquin de passe-passe";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Blagounettes C";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Encyclopedie A.F.";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Bible des maux";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Manuscrit ancien";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Manga";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Hentai";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Transformation";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Vidage";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Guide strategique";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Impenetrable";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Couardise";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Livre audio";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Duo";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Monotonie";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "BD";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Phylactere";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Case";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Omnimanach";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Renforcement";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Chagrin";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Mein Kampf";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Sublimation";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Mediocrisation";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Tome";
            InvACompl[i].Nom = "Cepatretregentinomicon";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 100;
            InvACompl[i].EffetType1 = "Buff";
            InvACompl[i].EffetDes1 = "Tretregenti";
            InvACompl[i].EffetType2 = "Debuff";
            InvACompl[i].EffetDes2 = "Patretregenti";
            InvACompl[i].EffetType3 = "Crit";
            InvACompl[i].EffetDes3 = "Mechancete";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Lancers

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Caillou";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Sable";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Shuriken";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Fabrication";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Boomerang";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Poids";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Ecraser";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Assiette";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Bouchee";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Grenade";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Aura d'expert";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Bombe";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Blitzball";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Chakram";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Fiole";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Alea";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Lance";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Disque";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Lancer rotatif";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Javelot";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Boumb";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Aura d'expert";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Rocher";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Bombe H";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Aura d'expert";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Molotov";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Fournaise";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Soleil levant";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Kazekiri";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Omega";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Triplette";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Menhir";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Enterrer";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Lancers";
            InvACompl[i].Nom = "Bombe a chantilly";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 100;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Feastplosion";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Armes à feu

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Pistolet a eau";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Aerosol";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Tuyau d'arrosage";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Airomir";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Ventilateur";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Crache-feu";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Flambloiement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Laser";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Pistolet";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Canon a T-Shirts";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Paintball";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Portal gun";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "InstaRickite";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Hole in one";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Aerosol napalm";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Flamboiement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Souffleuse";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Repousse";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Révolvert";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Karcher";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Magnum";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Lance-flammes";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Flamboiement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Gatling";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Laser Mk II";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Rayon";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Ambre spirituelle";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Coeur";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Floraison ardente";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Gunner";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Arme a feu";
            InvACompl[i].Nom = "Lanceur";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC - L";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 20;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Lancement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Frondes

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Boule de neige";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Balle de tennis";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Boing-boing";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Echardes";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Baseball";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Bonbons";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Balle rebondissante";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Boing-boing";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Bogue";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Piege piquant";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Gyroballe";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Clous";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Baballe";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Boule de billard";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Beurre noir";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Herisson";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Piege epineux";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Lettre";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Missive importante";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Bolas";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Immobilisation";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Cendres";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Souffle cendre";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Boule de bowling";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Porc-epic";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Piege dechirant";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Fruits";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Festin";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Mole";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Int";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Influence";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Oignon";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Degats extra";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Peggle";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Fever";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Chatons";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Rassemblement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Fronde";
            InvACompl[i].Nom = "Numero surprise";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 100;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Bingo";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Haches

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Hachette";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Dash";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Ruee";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Gache";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Casse-dalle";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Steak hache";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Casse-dalle";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Tomahawk";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Lancer";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Piolet";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Mache";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Bouchee";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Tache";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Tache";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Bache";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Lache";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Peur";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Seconde main";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Apache";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Pioche";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Arracheur";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Hache-viande";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Cash";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Sans le sou";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Crachat";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Lancer";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Hauteclaire";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Hacheur";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Hache-viande";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Paralhache";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Paralysie";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Fache";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Furie";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Hache du mal";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Mecahache";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Hache";
            InvACompl[i].Nom = "Dieu bovin";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 100;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Vache folle";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Boucliers

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Briques";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Targe";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Contre-offensive";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Sacoche";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Couvercle";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Bouclier urticant";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Alteration";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Ecu";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "CRS";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Cloison";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Coince";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Sac a main";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Ombrelle";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Bouclier rutilant";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Affliction";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Mur";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Wall";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Cartable";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Couvercle de poubelle";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Parapluie";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Ecu de paladin";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Great Wall";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Great Wall";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Valise";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Legionnaire";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Veritas";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Mur demoniaque";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Champ de force";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Devier";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Imbattable";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Toute-Puissance";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bouclier";
            InvACompl[i].Nom = "Rhinoshield";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 20;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Rhinoshield !";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Poutres

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Poteau";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Spectacle";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Bras";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Terreur";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Raquette";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Journal";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Buche";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Criquet";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Batte";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Spectacle";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Catalogue";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Guitare";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Masse d'armes";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Home-run";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Spectacle";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Pinata";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Surprise";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Pied de biche";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Muramasa";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Encreur";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Encrage";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Smash";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Smash final";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Catalogue de Noel";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Pinceau";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Peinture";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Tronc";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Pilier";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Nain";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Rassemblement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Masamune";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Degats extra";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Poutre";
            InvACompl[i].Nom = "Poutre de Bamako";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 20;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Bien membre";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Pugilats

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Gants de boxe";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Papattes";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Griffes de chat";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Degouligants";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Poings americains";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Hand wraps";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Recup";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Pitbull";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Marteaux de poignets";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Ecorchure";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Gants lubrifies";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Longles";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Poussee";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Pompons etincelants";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Excavation";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Dechirement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Tonfas";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Lames secretes";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Gants BDSM";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Wolvegriffes";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Vrillons a percussions";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Penetration";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Mains d'argent";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CàC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Croissants de lune";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "God Hands";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Degats extra";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Gants dynamite";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Aura d'expert";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Foreuses de poignets";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Forage";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Pugilats";
            InvACompl[i].Nom = "Double fists";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 100;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Double coup";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Bestipierres

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Poulet";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Escargot";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 1;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Marsouin";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Porcinet";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Reaper";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Vilaine faux";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Rosinante";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Morty";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Geez !";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Pangolin";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Crit";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Iris";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 2;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Barbazius";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cha";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Babamut";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Malboro";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Neo";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 3;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "La matrice";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Tigrou";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Donald";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 8;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Sans le sou";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Behemoth";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Egeon";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Sau";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Winnie";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 4;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Festin";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Rafale";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 16;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Picsou";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Pre";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Effet";
            InvACompl[i].EffetDes1 = "Coffre-fort";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Chat-boule";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Cons";
            InvACompl[i].Bonus = 32;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Sieste";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "Noir";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Esq";
            InvACompl[i].Bonus = 5;
            InvACompl[i].EffetType1 = "Crit";
            InvACompl[i].EffetDes1 = "Rassemblement";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Bestipierre";
            InvACompl[i].Nom = "MJ";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "Ins";
            InvACompl[i].Bonus = 100;
            InvACompl[i].EffetType1 = "Objet";
            InvACompl[i].EffetDes1 = "Remodelage";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Consommables
            //Soins

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Fruit";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 3 PV";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 10;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Eau";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 5 PV";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 20;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Biere";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 20 PV; Pre = 0 (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 40;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Steak";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 10 PV";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 40;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Pain";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 15 PV";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 60;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Larmes de cherubin";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Dissipe buffs/debuffs";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 60;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Remede mortel";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "50% de restaurer tous les PV/les mettre a 1";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 70;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Vin";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 30 PV; Pre = 0 (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 70;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Chartreuse";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 50% des PV Max; Pre = 0 (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 100;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Elixir";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 25 PV; dissipe buffs/debuffs";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 100;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Rosee d'Yggdrasil";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 10 PV à l'equipe";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 120;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Feuille d'Yggdrasil";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Reanime un allie avec 10 PV";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Spyrytus";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 100% des PV Max; Pre = 0 (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Soin";
            InvACompl[i].Nom = "Jouvenceau";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soigne 100% des PV Max";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Dégâts

            InvACompl[i].Type = "Degats";
            InvACompl[i].Nom = "Bombe";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inflige 5 degats";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 20;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Degats";
            InvACompl[i].Nom = "Portail";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inflige 5 degats et deplace sur une distance M au choix";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 50;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Degats";
            InvACompl[i].Nom = "Bombe au poivre";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inflige 15 degats";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 50;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Degats";
            InvACompl[i].Nom = "Cocktail molotov";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inflige 15 degats sur une zone S";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 100;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Degats";
            InvACompl[i].Nom = "Bombe intelligente";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inflige 15 degats sur une zone M";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Degats";
            InvACompl[i].Nom = "Trou de ver";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inflige 15 degats et deplace sur une distance M au choix";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 200;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Degats";
            InvACompl[i].Nom = "Armagebombe";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "S - M";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inflige 30 degats sur une zone L";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Degats";
            InvACompl[i].Nom = "Piege mortel";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "30% de reduire les PV a 1";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Buffs

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Vitamine C";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Mvt +1 (3t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 50;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Serum physiologique";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Pre garantie (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 150;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Smoothie";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Esq garantie (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 150;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Decoction de rapidite";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Vit +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Viagra";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Cons +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Sushi";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Int +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Trefle";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Cha +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Wyverne ivre";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Sau +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Traumatisme";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Ins +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Carotte doree";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Pre +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Adrenaline";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Esq +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "49-3";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Crit +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Sirop a la menthe";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "PV Max +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Parfum";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Soc +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Bottes des sept lieues";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "CaC";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Mvt +1 (permanent)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Buff";
            InvACompl[i].Nom = "Seringue d'ultra instinct";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC - L";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Se tp sur un ennemi et inflige un Crit; Esq garantie (1t); action impossible tour suivant";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2500;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Debuffs

            InvACompl[i].Type = "Debuff";
            InvACompl[i].Nom = "Bombe de colle";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Mvt -1 (3t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 50;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Debuff";
            InvACompl[i].Nom = "Somnifere";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "50% d'empecher l'ennemi d'agir (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 100;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Debuff";
            InvACompl[i].Nom = "Poignee de sable";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Pre nulle (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 150;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Debuff";
            InvACompl[i].Nom = "Eau savonneuse";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "CaC - S";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Esq nulle (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 150;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Debuff";
            InvACompl[i].Nom = "Bulle de temps";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Mvt -1 (3t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 100;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Debuff";
            InvACompl[i].Nom = "Berceuse";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "50% d'empecher l'ennemi d'agir (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Debuff";
            InvACompl[i].Nom = "Flash";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Pre nulle (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Debuff";
            InvACompl[i].Nom = "Squilification";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Esq nulle (1t)";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Debuff";
            InvACompl[i].Nom = "Le Pagne";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "CaC - M";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Empeche l'ennemi d'agir (1t); la prochaine attaque recue par l'ennemi est un Crit";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1500;
            InvACompl[i].EmplOccupe = true;
            i++;

            //Compétences

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Custom A";
            InvACompl[i].Rarete = "1";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "3 pts de stats a repartir";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Custom B";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "5 pts de stats a repartir";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion A";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse la Vit et la Cons";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion B";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse l'Int et la Cha";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion C";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse la Sau et l'Ins";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Defaitiste";
            InvACompl[i].Rarete = "2";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Crit +10 quand PV > 50%; Crit /2 quand PV < 50%";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion D";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse la Vit et l'Int";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion E";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse l'Int et la Sau";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion F";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse la Sau et la Vit";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion G";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse la Cons et la Cha";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion H";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse la Cha et l'Ins";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion I";
            InvACompl[i].Rarete = "3";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse l'Ins et la Cons";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 300;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Ecu";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Degats recus -10%";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Glaive";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Degats infliges +10%";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Custom C";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "10 pts de stat a repartir";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion X";
            InvACompl[i].Rarete = "4";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse la Pre et l'Esq";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 500;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Custom Z";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "20 pts de stats a repartir";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 2000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Inversion Z";
            InvACompl[i].Rarete = "5";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Inverse les PV Max et le Crit";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 1000;
            InvACompl[i].EmplOccupe = true;
            i++;

            InvACompl[i].Type = "Comp";
            InvACompl[i].Nom = "Icone destructrice";
            InvACompl[i].Rarete = "EX";
            InvACompl[i].Portee = "-";
            InvACompl[i].BonusType = "";
            InvACompl[i].Bonus = 0;
            InvACompl[i].EffetType1 = "";
            InvACompl[i].EffetDes1 = "Ameliore le de de degats d'un niveau";
            InvACompl[i].EffetType2 = "";
            InvACompl[i].EffetDes2 = "";
            InvACompl[i].EffetType3 = "";
            InvACompl[i].EffetDes3 = "";
            InvACompl[i].Prix = 3000;
            InvACompl[i].EmplOccupe = true;
            i++;
        }

        //Fonction initialisation Compétences

        public void initComp(ref Competence[] comps)
        {
            int i = 0;

            comps[i].Nom = "A couper le souffle";
            comps[i].Type = "Active";
            comps[i].Effet = "(Int/2)%; Immobilise ennemis rayon M autour de l'unite";
            comps[i].Nb_tours = 1;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Ad-or-ation";
            comps[i].Type = "Passive";
            comps[i].Effet = "Degats +1 par tranche de 500G possedes";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Agilite";
            comps[i].Type = "Passive";
            comps[i].Effet = "Permet d'attaquer une 2eme fois avec degats/2";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Aide mentale";
            comps[i].Type = "Active";
            comps[i].Effet = "Garantit la reussite d'une [Commande] alliee a distance S max";
            comps[i].Nb_tours = 1;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Alchimie";
            comps[i].Type = "Active";
            comps[i].Effet = "Int%; Fusionne deux consommables";
            comps[i].Nb_tours = 1;
            comps[i].CD = 1;
            i++;

            comps[i].Nom = "Alchimiracle";
            comps[i].Type = "Passive";
            comps[i].Effet = "Int +10; Double le nb d'objets crees par Alchimie";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Assistance";
            comps[i].Type = "Passive";
            comps[i].Effet = "Ajoute la moitie d'une stat off/def à une stat def/off en combat";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Autographe";
            comps[i].Type = "Active";
            comps[i].Effet = "Soc%; Garantit un Crit pour un allie";
            comps[i].Nb_tours = 1;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Benediction";
            comps[i].Type = "Active";
            comps[i].Effet = "Accorde une barriere a un allie qui absorbe Cha/5 degats";
            comps[i].Nb_tours = 0;
            comps[i].CD = 5;
            i++;

            comps[i].Nom = "Berserk";
            comps[i].Type = "Active";
            comps[i].Effet = "Double les degats recus et infliges jusqu'a reutilisation de la comp";
            comps[i].Nb_tours = 0;
            comps[i].CD = 2;
            i++;

            comps[i].Nom = "Brute maligne";
            comps[i].Type = "Passive";
            comps[i].Effet = "Crit +5";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Chasseur de tresors";
            comps[i].Type = "Passive";
            comps[i].Effet = "Cha/Ins +5";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Confiant";
            comps[i].Type = "Passive";
            comps[i].Effet = "Pre/Crit +5";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Custom A";
            comps[i].Type = "Passive";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Custom B";
            comps[i].Type = "Passive";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Custom C";
            comps[i].Type = "Passive";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Custom Z";
            comps[i].Type = "Passive";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Debut calme";
            comps[i].Type = "Passive";
            comps[i].Effet = "Pre +7/tour jusqu'a la fin du combat";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Decouvertes";
            comps[i].Type = "Passive";
            comps[i].Effet = "Int +5";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Decuplo";
            comps[i].Type = "Active";
            comps[i].Effet = "Int%; Double/Divise par 2 l'efficacite d'un consommable";
            comps[i].Nb_tours = 0;
            comps[i].CD = 1;
            i++;

            comps[i].Nom = "Defaitiste";
            comps[i].Type = "Passive";
            comps[i].Effet = "Crit +10 si PV > 50%; Crit /2 sinon";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Destabilisation";
            comps[i].Type = "Active";
            comps[i].Effet = "Int%; Nullifie l'Esq d'un ennemi a une distance L max";
            comps[i].Nb_tours = 1;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Devotion";
            comps[i].Type = "Active";
            comps[i].Effet = "Agit une fois a la place d'un allie";
            comps[i].Nb_tours = 1;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Distorsion";
            comps[i].Type = "Active";
            comps[i].Effet = "Permet a tous les allies d'agir deux fois";
            comps[i].Nb_tours = 1;
            comps[i].CD = 5;
            i++;

            comps[i].Nom = "Docteur cadavre";
            comps[i].Type = "Active";
            comps[i].Effet = "Int%; Réanime un allie mort au combat";
            comps[i].Nb_tours = 1;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Economies";
            comps[i].Type = "Passive";
            comps[i].Effet = "30% de conserver un objet apres utilisation";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Ecu";
            comps[i].Type = "Passive";
            comps[i].Effet = "Degats recus -10%";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Effluves nefastes";
            comps[i].Type = "Passive";
            comps[i].Effet = "Contre si attaque au CaC recue; Voir doc";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Epinogarde";
            comps[i].Type = "Passive";
            comps[i].Effet = "Renvoie 50% des degats au CaC recus";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Equilibre";
            comps[i].Type = "Passive";
            comps[i].Effet = "Int/Sau +3; Vit -1";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Errance";
            comps[i].Type = "Passive";
            comps[i].Effet = "Vit +3; Sau +2";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Et voici...";
            comps[i].Type = "Active";
            comps[i].Effet = "1D20; Allie a une distance S max; Voir doc";
            comps[i].Nb_tours = 1;
            comps[i].CD = 1;
            i++;

            comps[i].Nom = "Experience";
            comps[i].Type = "Passive";
            comps[i].Effet = "Vit/Sau +4; Crit +2";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Expertise critique";
            comps[i].Type = "Passive";
            comps[i].Effet = "Ajoute le Crit a la Pre et l'Esq";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Extase";
            comps[i].Type = "Passive";
            comps[i].Effet = "Crit +5 par tranche de 10 PV Max perdus";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Festin sanglant";
            comps[i].Type = "Passive";
            comps[i].Effet = "Regenere 20% des PV Max en eliminant un ennemi";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Frustration";
            comps[i].Type = "Passive";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Fouille";
            comps[i].Type = "Active";
            comps[i].Effet = "1D20; Utilisation impossible si inventaire plein";
            comps[i].Nb_tours = 1;
            comps[i].CD = 1;
            i++;

            comps[i].Nom = "Gaga";
            comps[i].Type = "Passive";
            comps[i].Effet = "Sau +10";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Gargarisation";
            comps[i].Type = "Passive";
            comps[i].Effet = "Crit +10 pour le combat en eliminant un ennemi";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Glaive";
            comps[i].Type = "Passive";
            comps[i].Effet = "Degats infliges +10%";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Hades";
            comps[i].Type = "Passive";
            comps[i].Effet = "Un sort critique a 33% de chances de one-shot";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Icone destructrice";
            comps[i].Type = "Passive";
            comps[i].Effet = "Ameliore le de de degats d'un niveau";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Idole";
            comps[i].Type = "Passive";
            comps[i].Effet = "Soc +10; Permet d'utiliser les Buffs";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Illusioniste";
            comps[i].Type = "Passive";
            comps[i].Effet = "Int +3; Esq +2";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Ingeniosite";
            comps[i].Type = "Passive";
            comps[i].Effet = "Multiplie l'argent gagne en fct du nb de tours a la fin du combat";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Intinct sauvage";
            comps[i].Type = "Passive";
            comps[i].Effet = "Sau +3; Esq +2";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion A";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse la Vit et la Cons";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion B";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse l'Int et la Cha";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion C";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse la Sau et l'Ins";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion D";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse la Vit et l'Int";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion E";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse l'Int et la Sau";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion F";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse la Sau et la Vit";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion G";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse la Cons et la Cha";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion H";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse la Cha et l'Ins";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion I";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse l'Ins et la Cons";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion X";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse la Pre et l'Esq";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Inversion Z";
            comps[i].Type = "Passive";
            comps[i].Effet = "Inverse les PV Max et le Crit";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Investissement";
            comps[i].Type = "Active";
            comps[i].Effet = "Permet de depenser 400G pour obtenir 1 pt de comp";
            comps[i].Nb_tours = 0;
            comps[i].CD = 1;
            i++;

            comps[i].Nom = "Karma";
            comps[i].Type = "Passive";
            comps[i].Effet = "Cha% de survivre a une attaque fatale (3 fois / combat)";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Larmes factices";
            comps[i].Type = "Passive";
            comps[i].Effet = "(Soc/2)%; Esq garantie pour une attaque au CaC";
            comps[i].Nb_tours = 0;
            comps[i].CD = 2;
            i++;

            comps[i].Nom = "Leadership";
            comps[i].Type = "Active";
            comps[i].Effet = "Mvt +1 pour tous les allies";
            comps[i].Nb_tours = 1;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Mefiance";
            comps[i].Type = "Passive";
            comps[i].Effet = "Empeche l'unite et l'ennemi d'esquiver les attaques";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Mercurochrome";
            comps[i].Type = "Active";
            comps[i].Effet = "Recupere 10PV immediatement apres avoir recu des degats";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Moitie-moitie";
            comps[i].Type = "Active";
            comps[i].Effet = "Utilise un objet sur x allies a une distance M max en divisant son efficacite par x";
            comps[i].Nb_tours = 0;
            comps[i].CD = 1;
            i++;

            comps[i].Nom = "Morphine hormonale";
            comps[i].Type = "Active";
            comps[i].Effet = "Divise les degats recus par 2 (2t) puis les double (1t)";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Nouveau chapeau";
            comps[i].Type = "Passive";
            comps[i].Effet = "(Int/2)% de creer une arme de rarete superieure en utilisant Tada !";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Oubliettes";
            comps[i].Type = "Active";
            comps[i].Effet = "(Soc/2)%; vire un ennemi du combat et recupere ses objets";
            comps[i].Nb_tours = 0;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Overkill";
            comps[i].Type = "Passive";
            comps[i].Effet = "Ameliore le de de degats d'un niveau";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Pardon";
            comps[i].Type = "Passive";
            comps[i].Effet = "Permet de relancer un de de Soc (1 fois / periode|tour)";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Parlotte";
            comps[i].Type = "Passive";
            comps[i].Effet = "Esquive la premiere attaque recue";
            comps[i].Nb_tours = 0;
            comps[i].CD = 5;
            i++;

            comps[i].Nom = "Piete";
            comps[i].Type = "Passive";
            comps[i].Effet = "Cha +10";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Plein aux as";
            comps[i].Type = "Passive";
            comps[i].Effet = "Vit/Sau +5; argent gagne +50%";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Polyvalence";
            comps[i].Type = "Passive";
            comps[i].Effet = "Portee d'armes +1";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Priere";
            comps[i].Type = "Passive";
            comps[i].Effet = "Cha +3; Vit +2";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Provocation";
            comps[i].Type = "Passive";
            comps[i].Effet = "Recoit toutes les attaques recues dans un rayon S autour de soi";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Quatre verites";
            comps[i].Type = "Passive";
            comps[i].Effet = "Int +10; Permet d'utiliser les Debuffs";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Reflexion";
            comps[i].Type = "Passive";
            comps[i].Effet = "Int +3; Soc +2";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Renfermement";
            comps[i].Type = "Passive";
            comps[i].Effet = "Int +20; Cons/Ins -5";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Renforcement";
            comps[i].Type = "Passive";
            comps[i].Effet = "Cons/Cha/Ins +5";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Resistance mythique";
            comps[i].Type = "Passive";
            comps[i].Effet = "Immunite aux (De)Buffs/Soins/Tomes";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Romancier";
            comps[i].Type = "Passive";
            comps[i].Effet = "Int/Sau +3";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Rustre";
            comps[i].Type = "Passive";
            comps[i].Effet = "Permet d'utiliser la stat de Sau à la place du Soc";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Scalper";
            comps[i].Type = "Passive";
            comps[i].Effet = "10% de one-shot l'ennemi en le soignant";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Serment d'Hippocrate";
            comps[i].Type = "Passive";
            comps[i].Effet = "Proba de soigner en tapant (100% allies; 50% ennemis)";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Soif de sang";
            comps[i].Type = "Passive";
            comps[i].Effet = "Si ennemi elimine, peut rattaquer sur une distance S";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Solitaire";
            comps[i].Type = "Passive";
            comps[i].Effet = "Crit +5; Soc -5";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Soudoiement";
            comps[i].Type = "Active";
            comps[i].Effet = "Rattrape un jet de Soc en payant la diff * 10G";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Spectacle etendu";
            comps[i].Type = "Passive";
            comps[i].Effet = "Etend l'att sur un rayon S autour de la victime";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Substitution";
            comps[i].Type = "Active";
            comps[i].Effet = "Int%; change une arme en une autre";
            comps[i].Nb_tours = 0;
            comps[i].CD = 1;
            i++;

            comps[i].Nom = "Subordonne";
            comps[i].Type = "Active";
            comps[i].Effet = "Se met en duo avec un allie pour partager les degats";
            comps[i].Nb_tours = 0;
            comps[i].CD = 2;
            i++;

            comps[i].Nom = "Switch";
            comps[i].Type = "Passive";
            comps[i].Effet = "Choisit la stat a utiliser pour att/def selon le type d'att";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Symbiose";
            comps[i].Type = "Passive";
            comps[i].Effet = "Sau/Crit +5";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Tada !";
            comps[i].Type = "Active";
            comps[i].Effet = "Att et trans un ennemi en arme si vaincu";
            comps[i].Nb_tours = 0;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Traumas";
            comps[i].Type = "Active";
            comps[i].Effet = "Int%; nullifie la Pre d'un ennemi a distance L max; Crit possible";
            comps[i].Nb_tours = 0;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Vague de peur";
            comps[i].Type = "Active";
            comps[i].Effet = "Mvt -1 a tous les ennemis";
            comps[i].Nb_tours = 0;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Vener";
            comps[i].Type = "Passive";
            comps[i].Effet = "PV/Crit +5";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            //Armes

            comps[i].Nom = "Bouchee";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Repas";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Fabrication";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Casse-dalle";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Multidextre";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Navigation";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Verrouillage";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 1;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Multiusage";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Hache-viande";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Gene";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 1;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Reglages";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Toute-puissance";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Divinisme";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 2;
            comps[i].CD = 4;
            i++;

            comps[i].Nom = "Marchandage";
            comps[i].Type = "Passive";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Rengorgement";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Calme";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Rigidite";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Fragilite";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Tilt";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "I.M.F";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Chance";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Gacha";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Degout";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Rapidite";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 1;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Pifometre";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 1;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Promenade";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Transformation";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Vidage";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Impenetrable";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Couardise";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Duo";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Monotonie";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Phylactere";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Case";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Renforcement";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Chagrin";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Sublimation";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Mediocrisation";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 3;
            i++;

            comps[i].Nom = "Tretregenti";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 1;
            comps[i].CD = 5;
            i++;

            comps[i].Nom = "Patretregenti";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 1;
            comps[i].CD = 5;
            i++;

            comps[i].Nom = "Festin";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 3;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Sans le sou";
            comps[i].Type = "Passive";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Bien membre";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 2;
            comps[i].CD = 2;
            i++;

            comps[i].Nom = "Recup";
            comps[i].Type = "Passive";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Coffre-fort";
            comps[i].Type = "Passive";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Sieste";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 0;
            i++;

            comps[i].Nom = "Remodelage";
            comps[i].Type = "Active";
            comps[i].Effet = "";
            comps[i].Nb_tours = 0;
            comps[i].CD = 2;
            i++;
        }
    }
}
