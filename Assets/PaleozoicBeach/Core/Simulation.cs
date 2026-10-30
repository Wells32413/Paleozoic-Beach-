using System;
using System.Collections.Generic;

namespace PaleozoicBeach.Core
{
    [Serializable] public sealed class Species
    {
        public string id, name, habitat, uncertainty;
        public float oldestMa, youngestMa;
        public bool enabled;
        public bool Occurs(float age, string zone) => enabled && age <= oldestMa && age >= youngestMa && zone == habitat;
    }
    [Serializable] public sealed class Discovery { public string speciesId; public float quality; public int photographs; }
    [Serializable] public sealed class Progress
    {
        public int version = 1;
        public int funds = 1000, expeditions, exhibits;
        public bool tutorialComplete;
        public List<Discovery> discoveries = new List<Discovery>();
        public int ResearchCount => discoveries.Count;
        public bool BuildExhibit()
        {
            if (exhibits >= ResearchCount || funds < 300) return false;
            funds -= 300; exhibits++; return true;
        }
        public int OperatePark()
        {
            int net = exhibits * 100;
            funds += net; return net;
        }
    }
    public sealed class Expedition
    {
        public const float Duration = 480;
        public const int Cost = 250;
        public float Elapsed { get; private set; }
        public float Battery { get; private set; } = 100;
        public float Oxygen { get; private set; } = 100;
        public float Hull { get; private set; } = 100;
        public bool Finished { get; private set; }
        public bool Survived => Finished && Hull > 0 && Oxygen > 0 && Battery > 0;
        public readonly List<Discovery> Research = new List<Discovery>();
        private bool settled;
        public static Expedition Launch(Progress progress)
        {
            if (progress.tutorialComplete && progress.funds < Cost) return null;
            if (progress.tutorialComplete) progress.funds -= Cost;
            return new Expedition();
        }
        public void Tick(float seconds, bool thrust)
        {
            if (float.IsNaN(seconds) || float.IsInfinity(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            if (Finished) return;
            float dt = Math.Min(seconds, Duration - Elapsed);
            Elapsed += dt;
            Battery = Math.Max(0, Battery - dt * (thrust ? .16f : .08f));
            Oxygen = Math.Max(0, Oxygen - dt * .12f);
            Finished = Elapsed >= Duration || Battery <= 0 || Oxygen <= 0 || Hull <= 0;
        }
        public void Damage(float amount)
        {
            if (amount < 0 || float.IsNaN(amount) || float.IsInfinity(amount)) throw new ArgumentOutOfRangeException(nameof(amount));
            if (Finished) return;
            Hull = Math.Max(0, Hull - amount);
            if (Hull <= 0) Finished = true;
        }
        public bool Photograph(Species species, float age, string habitat, float distance, float alignment, bool visible)
        {
            if (Finished || species == null || !species.Occurs(age, habitat) || !visible || float.IsNaN(distance) || float.IsInfinity(distance) || float.IsNaN(alignment) || distance < 1 || distance > 35 || alignment < .94f || alignment > 1) return false;
            float quality = Math.Max(.1f, Math.Min(1, alignment * (1 - distance / 45)));
            var record = Research.Find(x => x.speciesId == species.id);
            if (record == null) { record = new Discovery { speciesId = species.id }; Research.Add(record); }
            record.quality = Math.Max(record.quality, quality); record.photographs++;
            return true;
        }
        public void Settle(Progress progress)
        {
            if (!Finished || settled) return;
            settled = true; progress.expeditions++;
            if (!Survived) return;
            progress.tutorialComplete = true;
            foreach (var item in Research)
            {
                var saved = progress.discoveries.Find(x => x.speciesId == item.speciesId);
                if (saved == null) { saved = new Discovery { speciesId = item.speciesId }; progress.discoveries.Add(saved); progress.funds += 200; }
                saved.quality = Math.Max(saved.quality, item.quality); saved.photographs += item.photographs;
            }
        }
    }
}
