﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Screens.Play.HUD
{
    /// <summary>
    /// Displays a single-line horizontal auto-sized flow of mods. For cases where wrapping is required, use <see cref="ModFlowDisplay"/> instead.
    /// </summary>
    public partial class ModDisplay : CompositeDrawable, IHasCurrentValue<IReadOnlyList<Mod>>
    {
        private ExpansionMode expansionMode = ExpansionMode.ExpandOnHover;

        public ExpansionMode ExpansionMode
        {
            get => expansionMode;
            set
            {
                if (expansionMode == value)
                    return;

                expansionMode = value;

                if (IsLoaded)
                {
                    if (expansionMode == ExpansionMode.AlwaysExpanded || (expansionMode == ExpansionMode.ExpandOnHover && IsHovered))
                        expand();
                    else if (expansionMode == ExpansionMode.AlwaysContracted || (expansionMode == ExpansionMode.ExpandOnHover && !IsHovered))
                        contract();
                }
            }
        }

        private readonly BindableWithCurrent<IReadOnlyList<Mod>> current = new BindableWithCurrent<IReadOnlyList<Mod>>(Array.Empty<Mod>());

        public Bindable<IReadOnlyList<Mod>> Current
        {
            get => current.Current;
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                current.Current = value;
            }
        }

        private bool showExtendedInformation;

        public bool ShowExtendedInformation
        {
            get => showExtendedInformation;
            set
            {
                showExtendedInformation = value;
                foreach (var icon in iconsContainer)
                    icon.ShowExtendedInformation = value;
            }
        }

        private readonly FillFlowContainer<ModIcon> iconsContainer;

        public ModDisplay(bool showExtendedInformation = true)
        {
            this.showExtendedInformation = showExtendedInformation;

            AutoSizeAxes = Axes.Both;

            InternalChild = iconsContainer = new ReverseChildIDFillFlowContainer<ModIcon>
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Current.BindValueChanged(updateDisplay, true);

            switch (expansionMode)
            {
                case ExpansionMode.AlwaysExpanded:
                    expand(0);
                    break;

                case ExpansionMode.AlwaysContracted:
                    contract(0);
                    break;

                case ExpansionMode.ExpandOnHover:
                    if (IsHovered)
                        expand(0);
                    else
                        contract(0);
                    break;
            }
        }

        private void updateDisplay(ValueChangedEvent<IReadOnlyList<Mod>> mods)
        {
            iconsContainer.Clear();

            foreach (Mod mod in mods.NewValue.AsOrdered())
                iconsContainer.Add(new ModIcon(mod, showExtendedInformation: showExtendedInformation) { Scale = new Vector2(0.6f) });
        }

        private void expand(double duration = 500)
        {
            if (ExpansionMode != ExpansionMode.AlwaysContracted)
                iconsContainer.TransformSpacingTo(new Vector2(5, 0), duration, Easing.OutQuint);
        }

        private void contract(double duration = 500)
        {
            if (ExpansionMode != ExpansionMode.AlwaysExpanded)
                iconsContainer.TransformSpacingTo(new Vector2(-25, 0), duration, Easing.OutQuint);
        }

        protected override bool OnHover(HoverEvent e)
        {
            expand();
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            contract();
            base.OnHoverLost(e);
        }
    }

    public enum ExpansionMode
    {
        /// <summary>
        /// The <see cref="ModDisplay"/> will expand only when hovered.
        /// </summary>
        ExpandOnHover,

        /// <summary>
        /// The <see cref="ModDisplay"/> will always be expanded.
        /// </summary>
        AlwaysExpanded,

        /// <summary>
        /// The <see cref="ModDisplay"/> will always be contracted.
        /// </summary>
        AlwaysContracted,
    }
}
