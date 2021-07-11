using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RazzleDazzle
{
    // Token: 0x0200001D RID: 29
    public class LordJob_PerformComedySet : LordJob_Performance
    {
        // Token: 0x0400002C RID: 44
        public bool punchline;

        // Token: 0x0600009D RID: 157 RVA: 0x0000486C File Offset: 0x00002A6C
        public LordJob_PerformComedySet()
        {
        }

        // Token: 0x0600009E RID: 158 RVA: 0x00004874 File Offset: 0x00002A74
        public LordJob_PerformComedySet(Building_Performance venue)
        {
            this.venue = venue;
        }

        // Token: 0x0600009F RID: 159 RVA: 0x00004884 File Offset: 0x00002A84
        protected override float GetFinalQuality()
        {
            var num = (float) GetQualityModifier(QualityUtility.GenerateQualityCreatedByPawn(Lead, SkillDefOf.Social));
            var num2 = (float) GetQualityModifier(
                QualityUtility.GenerateQualityCreatedByPawn(Lead, SkillDefOf.Artistic));
            return Lead.health.capacities.GetLevel(PawnCapacityDefOf.Talking) * Rand.Range(num, num + num2);
        }

        // Token: 0x060000A0 RID: 160 RVA: 0x000048E8 File Offset: 0x00002AE8
        public override string GetMessageForQualityLevel(float fQ)
        {
            string text;
            if (fQ < 4f)
            {
                text = "RAZ_ComedyFinishedHated";
            }
            else if (fQ < 12f)
            {
                text = "RAZ_ComedyFinishedGood";
            }
            else if (fQ < 20f)
            {
                text = "RAZ_ComedyFinishedGreat";
            }
            else
            {
                text = "RAZ_ComedyFinishedPeerless";
            }

            return "RAZ_ComedyFinished".Translate(text.Translate());
        }

        // Token: 0x060000A1 RID: 161 RVA: 0x00004940 File Offset: 0x00002B40
        public override ThoughtDef GetThoughtForQualityLevel(float fQ)
        {
            ThoughtDef result;
            if (fQ < 4f)
            {
                result = ThoughtDefOfRazzleDazzle.AttendedBadGig;
            }
            else if (fQ < 12f)
            {
                result = ThoughtDefOfRazzleDazzle.AttendedGig;
            }
            else if (fQ < 20f)
            {
                result = ThoughtDefOfRazzleDazzle.AttendedGoodGig;
            }
            else
            {
                result = ThoughtDefOfRazzleDazzle.AttendedAmazingGig;
            }

            return result;
        }

        // Token: 0x060000A2 RID: 162 RVA: 0x00004984 File Offset: 0x00002B84
        protected override LordToil GetPerformanceLordToil()
        {
            return new LordToil_ComedySet(Lead, Venue);
        }
    }
}