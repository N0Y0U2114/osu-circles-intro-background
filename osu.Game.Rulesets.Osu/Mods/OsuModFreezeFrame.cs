using System;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModFreezeFrame : ModWithVisibilityAdjustment, IHidesApproachCircles, IApplicableToDrawableRuleset<OsuHitObject>
    {
        public override string Name => "Freeze frame";

        public override string Acronym => "FF";

        public override double ScoreMultiplier => 1;

        public override LocalisableString Description => "Burn the notes into your memory";

        public override ModType Type => ModType.Fun;

        public override IconUsage? Icon => FontAwesome.Solid.Camera;

        public override Type[] IncompatibleMods => new[] { typeof(OsuModTarget), typeof(OsuModStrictTracking) };

        [SettingSource("Beat divisor")]
        public BindableFloat BeatDivisor { get; } = new BindableFloat(1)
        {
            MinValue = .25f,
            MaxValue = 5,
            Precision = .25f
        };

        public void ApplyToDrawableRuleset(DrawableRuleset<OsuHitObject> drawableRuleset)
        {
            (drawableRuleset.Playfield as OsuPlayfield)?.FollowPoints.Hide();
        }

        public override void ApplyToBeatmap(IBeatmap beatmap)
        {
            base.ApplyToBeatmap(beatmap);

            foreach (var obj in beatmap.HitObjects.OfType<OsuHitObject>())
            {
                var point = beatmap.ControlPointInfo.TimingPointAt(obj.StartTime);
                double newPreempt = obj.TimePreempt + (obj.StartTime +5) % (point.BeatLength * BeatDivisor.Value);
                applyFadeInAdjustment(obj);

                void applyFadeInAdjustment(OsuHitObject osuObject)
                {
                    osuObject.TimePreempt = newPreempt;
                    foreach (var nested in osuObject.NestedHitObjects.OfType<OsuHitObject>())
                        applyFadeInAdjustment(nested);
                }
            }
        }

        protected override void ApplyIncreasedVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }

        protected override void ApplyNormalVisibilityState(DrawableHitObject hitObject, ArmedState state) => applyFrozenState(hitObject, state);

        private void applyFrozenState(DrawableHitObject drawableObject, ArmedState state)
        {
        }
    }
}

