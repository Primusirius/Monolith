// SPDX-FileCopyrightText: 2022 EmoGarbage404
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2024 Whatstone
// SPDX-FileCopyrightText: 2025 bitcrushing
//
// SPDX-License-Identifier: MPL-2.0

using Content.Shared.Instruments;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Player;

namespace Content.Server.Instruments;

public sealed class SwappableInstrumentSystem : EntitySystem
{
    [Dependency] private readonly SharedInstrumentSystem _sharedInstrument = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SwappableInstrumentComponent, GetVerbsEvent<AlternativeVerb>>(AddStyleVerb);
    }

    private void AddStyleVerb(EntityUid uid, SwappableInstrumentComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess || component.InstrumentList.Count <= 1)
            return;

        if (!TryComp<InstrumentComponent>(uid, out var instrument))
            return;

        var priority = 0;
        foreach (var entry in component.InstrumentList)
        {
            AlternativeVerb selection = new()
            {
                Text = entry.Key,
                Category = VerbCategory.InstrumentStyle,
                Priority = priority,
                Act = () =>
                {
                    _sharedInstrument.SetInstrumentProgram(uid, instrument, entry.Value.Item1, entry.Value.Item2);
                    _popup.PopupEntity(Loc.GetString("swappable-instrument-component-style-set", ("style", entry.Key)),
                        args.User, args.User);
                }
            };

            priority--;
            args.Verbs.Add(selection);
        }
    }
}
