# Organic Integration Playtest Issues

This file tracks playtest reports and pending changes for the Organic Integration mod.

Statuses:
- `Open`: reported and not yet addressed.
- `Next Test`: patched or planned for the next build/test pass.
- `Fixed`: implemented and verified.
- `Deferred`: accepted, but intentionally not part of the current pass.

## Open

### OI-001 Narrative Collision: BCI Route Into Vanilla Anthroneuroweave

- Status: `Open`
- Reported: 2026-06-24
- Found during: First playthrough after completing `Researching BCIs`
- Area: Machine project flow and player-facing text
- Related IDs: `OI_ResearchingBCIs`, `Ch2_MIN_DesignHumanCompatibleNeuroweave`

Observed:
After completing `Researching BCIs`, the next project is vanilla `Design Human Compatible Neuroweave`. Its player-facing text assumes the player is designing Anthroneuroweave specifically to inject it into Yishi Wellness's supply chain. That framing is correct for the vanilla FedCorp/Yishi route, but it collides with the new BCI onramp.

Expected:
The BCI onramp should frame Anthroneuroweave as a direct compatibility-layer research path from TMI's self-therapy insight into human-compatible neuroweave. The Yishi/FedCorp framing should remain available for the vanilla route.

Proposed change:
- Create a modded copy of the vanilla design project, likely `OI_DesignHumanCompatibleNeuroweave`.
- Rewrite the copied project's text around BCIs, neuroweave ancestry, and direct human compatibility work.
- Point `OI_ResearchingBCIs` to the copied project instead of `Ch2_MIN_DesignHumanCompatibleNeuroweave`.
- Make the copied project satisfy downstream Integration gates that currently depend on `Ch2_MIN_DesignHumanCompatibleNeuroweave`.
- Prevent duplicate/replayed design work if the player later reaches the vanilla FedCorp/Yishi route. Options include marking the vanilla project complete when the copied project completes, or gating the vanilla project so it is skipped after the modded copy.
- Preserve vanilla Yishi sale, Rebel Anthroneuroweave, Syndicate BrainPal, and BrainPal sales content.

Notes:
This may be cleaner than forcing vanilla text to serve two incompatible contexts. The implementation should avoid breaking the existing vanilla route or the mod's later microbot-to-nanobot arc.

## Next Test

No items yet.

## Fixed

No items yet.

## Deferred

No items yet.
