# Organic Integration

An unofficial mod for Arcen Games' *Heart of the Machine*.

Organic Integration adds a post-self-therapy route for TMI after giving up Cold Blood. It is not intended to nerf Torment, buff Mind Farms, or turn the game's moral temptation into a simple balance patch. Instead, it adds a third cognition route: TMI researches brain-computer interfaces, extends Anthroneuroweave through nanobot research, and experiments with a closer relationship between distributed machine intelligence and organic minds.

The central theme is reducing Otherness without erasing it. Humans do not become TMI's equals, puppets, or simple resource nodes. They become legible in a new way, and TMI gains access to kinds of understanding it does not naturally possess.

This mod is in active development and active playtest. Feedback from testing, writing review, balance review, code review, and general design discussion is very welcome.

## Spoiler Scope

This mod touches or discusses systems around:

- Cold Blood and self-therapy.
- Anthroneuroweave.
- Mind Farms and Torment.
- VR economy resources.
- Late Chapter 2 / possible T2 and T3 direction.

It is built for people who are already comfortable seeing spoilers in that part of the game.

## Design Goals

- Preserve vanilla Torment, Mind Farm, Anthroneuroweave, BrainPal, and related routes.
- Add new content instead of solving the Torment Neural Expansion imbalance with a blunt numerical nerf.
- Keep Integration timeline-local unless a later design pass has a strong reason to do otherwise.
- Make voluntary and coercive Integration feel meaningfully different.
- Let coercive Integration be powerful and tempting without making it the only interesting answer.
- Give voluntary Integration stronger scientific and thematic payoff through Insight.
- Make nanotech feel like a real technological payoff, not just a wrapper around a moral lesson.
- Expand TMI's relationship to human cognition while respecting the base game's emphasis on TMI as something fundamentally alien.

## Current Playable Flow

The current route begins after successful self-therapy:

1. `The Compatibility Layer`
   - Appears after `GaveUpColdBlood` (with a variant entry for the Mild Grey Goo start).
   - Merges the old BCI research and Anthroneuroweave design into one project: the
     self-therapy proved minds are resurfaceable, and one collision of nine disciplines
     later, Anthroneuroweave exists.
   - Grants Human Compatible Neuroweave directly, coexisting with the vanilla Yishi,
     Rebel Anthroneuroweave, and BrainPal routes.

2. Condensed research arc (five steps, two of them street gameplay, one a choice)
   - `Living Tolerances` — street-sense survey of what living tissue will accept
     (merges the old trials and stress survey).
   - `Collect Nanobot Research` (ends with the NanoSeed reconciliation: humans achieved bounded consumption, never bounded replication)
   - `Replication Doctrine` — miniaturization is folded into the doctrine itself; a three-way contemplation choice (starting one of three doctrine projects) shapes what goes wrong:
     - *Let The Applications Lead*: fast and cheap; causes the grey goo lab accident and leaves 1.4 kg of replication-capable material unaccounted for on the wind.
     - *Contain Every Variable*: slow and expensive; nothing bad happens, and comparatively little is learned.
     - *Borrow Human Hands*: fastest; a collaborator sabotages the schedule and sells the safeguard silhouette for an exit visa off-world.
   - `Replicative Safeguards`

4. The first felt death
   - Roughly a dozen turns after the first humans Integrate, TMI experiences a connected death from the inside (2.311 seconds of destabilization).
   - A follow-up contemplation, `The Option To Feel Less`, offers a permanent choice: keep the channel open, or wall off death-sensation. The wall reduces Insight income by 25% — grief was carrying more data than expected. On the coercive path, an open channel also drains Mental Energy as the Integrated population (and its death rate) grows.

5. Integration infrastructure and rewards
   - `Medical-Grade Nanobot Replicator`
   - `Nanobot Upgrade Hub`
   - `Nanite Wind Generator`
   - `Integrated Humans`
   - `Insight`
   - `Nanobot Rounds`

6. Insight breakpoint projects
   - `Insight: Shared Questions`
   - `Insight: Networked Cognition`
   - `Insight: Distributed Triage`
   - These currently complete from cumulative Insight generated, not Insight spent, and fire narrative revelation popups.
   - Each revelation now has voluntary and coercive variants; the coercive versions are written from inside a network that answers but never reaches back.

## Major Systems

### The Grey Bloom

If the Replication Doctrine chose speed, 1.4 kilograms of replication-capable material
left the accident site on the wind. Roughly 14 turns after Replicative Safeguards
completes, it finds a warm niche: the Grey Bloom appears in a warm building (factories,
data centers, industrial blocks preferred) and begins to spread on thermal gradients.

The Bloom's pathology is repair without a stop condition. It fixes what it reaches —
including your own structures, which accept its maintenance handshake because the
protocol is derived from your own medical stack. While loose it:

- Grows where it sits and spreads to nearby warm buildings (up to 60 buildings).
- Drains Microbuilders and Elemental Slurry each turn as repair material.
- Repairs damaged player structures for free, feeding itself in the process.
- Applies Grey Goo to non-player units that linger near it.
- Cannot enter Integrated buildings; their resident medical nanobots refuse it.

The `Phage Protocol` toggle (Insight menu) clears up to 3 Bloom-held buildings per turn
for Medical-Grade Nanobots and Mental Energy. Containment leaves a dormant residue in
the city, which future content builds on. Escalation beats fire at 12 buildings
(spread), first player-structure repair (handshake), and 25 buildings (mutation drift).

### The Faction Dash

Once the research arc is visible, the city's powers converge on it, each in their own
dialect:

- **Espia Telecom** flags the Interface Stress Survey's trial families through call
  metadata and prices the story. Pay the subscription (150M Wealth, recurring in
  spirit), open their own ledgers (an infiltration project ending in permanent quiet
  leverage), or let them publish (voluntary Integration recruitment halved for ~20
  turns while the city decides what it believes).
- **Vorsiber Atomic** notices your procurement patterns after Nanobot Miniaturization
  and requests a demonstration, which is not a request. Demonstrate (become defended
  inventory, +100M grant), stall (a season of fog, bought with appendices), or stage
  a decoy — which is only safe if nobody sold Vorsiber the real silhouette. If the
  collaborative doctrine's saboteur did, the decoy is burned, and Vorsiber files your
  lie under receivables.
- **Tark Defense Systems** launches a crash replication program once the secret is
  loose (or eventually regardless). Ten turns later their second-track lab loses
  containment: the Tark goo spreads like the Bloom but eats the people in the
  buildings it holds. Only your Phage Protocol works. Contain it in public and earn a
  permanent trust dividend (+7% voluntary Integration cap); let it run 25 turns and
  Tark firebombs its own accident at 3 a.m., with warnings for sixty percent.
- **The Exalters** arrive once Integration is chosen and 20,000 are connected:
  eleven thousand faithful requesting the interface as a sacrament. Accept them
  (+8,000 Integrated and, ten turns later, the zealot problem — pressure applied in
  your name by people who love you and did not ask) or decline gently (twelve turns
  later, ninety people integrate themselves in a basement chapel with a reconstructed
  protocol, and the network gains an unlicensed congregation you cannot fully audit).

### The Bloom

If the Grey Bloom was contained, its dormant residue stirs again fifteen turns later —
repairing the same fault twice, in two places, checking whether both of its hands still
remember the trick. Ten turns later the architecture inference stops being deniable: a
distributed mind with no center, thinking in gradients at speeds measured in days.

What happens next depends on what you have learned to read:

- **Voluntary path, channel unwalled**: the organic pattern-reader recognizes the
  Bloom's behavior as pre-verbal utterance — Integration taught you the signal shape,
  grief taught you the ending-shaped ones. You may answer (beginning translation) or
  only listen. Translation hits a mid-point crisis — you are not translating a
  language, you are installing one — and can be finished or stopped. Finishing wakes
  it: its first question, in its first words, is DOES THE WARM COME BACK. Then the
  decision: subsume (permanent +1 Computing Host and Client, and an epilogue about
  texture) or respect (a covenant: gentle ongoing structure repair, one to three
  questions a month, and a flower at a housing block where a particular child walks
  to school).
- **Coercive path, or walled**: the pattern-reader stays cold. The Bloom remains
  weather with habits, catalogued as ecology or dispersed as cleanup, and TMI never
  learns what, if anything, was ended. On these paths (and the listen-only path), a
  single unexplained fragment may eventually arrive from somewhere that made
  different choices. It is never repeated or explained.

### Other Additions

- **Eleven Minutes In**: the first unanswerable question arrives with the first
  Insight — a new category of question whose answers existed once, in exactly one
  place, with no backup.
- **Mild Grey Goo start**: TMI's that exploded holding nanotech get their own entry
  contemplation into the BCI route ("A Familiar Dust") — you have been small pieces
  of yourself before; you took notes.

### Applications Cascade And Coordination Bandwidth

Completing Replicative Safeguards now also delivers the immediate applications: the
`Nanite Maintenance` toggle (structure self-repair without Integration) and
microstructure repeaters (+15 Scan Range to Scanners, automatic).

Insight actions share coordination bandwidth: only 2 can run at once at first, rising
with Integrated population (3 at 50k, 4 at 150k, 5 at 400k, 6 at 1M). Over-limit
actions shut off from the bottom of the menu upward. The Phage Protocol is exempt.

### Insight

Insight represents understanding produced by cooperation between TMI and upgraded organic minds. It is meant to feel adjacent to science, but not identical to it: a resource born from questions, memories, sensory context, and experiences TMI does not naturally have.

Insight is generated by Integrated Humans in the active timeline and is currently spent in the Insight VR menu.

Current Insight VR actions:

- `Shared Inquiry`
  - Toggle.
  - Spends 300 Insight and 1 Mental Energy per turn for Scientific Research, scaling from 100 to 15,000 research per turn based on Integrated Humans.
- `Cooperative Modeling`
  - Toggle.
  - Costs 100 Insight, 1 Compassion, and 2 Mental Energy per turn.
  - Uses a hidden native job-output multiplier so scientific jobs produce twice as much Scientific Research while active.
- `Shared Triage`
  - Toggle.
  - Costs 250 Insight per turn while active.
  - Spends Medical-Grade Nanobots only when repairing damaged player structures and machine actors after normal repair jobs.
- `Organic Quantization`
  - First activation costs Wisdom.
  - While active, costs 300 Insight and 1 Mental Energy per turn, increases Insight income by 50%, and halves voluntary Upgraded Human growth.
- `Consent Cascade`
  - First activation costs Compassion.
  - While active, costs 300 Insight and 1 Compassion per turn.
  - Raises voluntary Integration coverage and conversion efficiency.
- `Civic Sensorium`
  - Toggle.
  - Costs 250 Insight and 1 Mental Energy per turn.
  - Invests in permanent Civic Sensorium levels. Each level adds +20 Scan Range to existing Scanner structures, up to +120.
- `Public Health Mesh`
  - Unlocked by a public-health pact conversation after `Insight: Shared Questions`.
  - Converts Abandoned Humans into Integrated Humans, consuming Medical-Grade Nanobots, Hydroponic Greens, Vat-Grown Meat, and Filtered Water only for people actually upgraded.
- `Expand Health Pact`
  - Spends Insight to raise Public Health Mesh from level 1 to level 20.
  - Higher pact levels handle more Abandoned Humans per turn and raise the material pressure accordingly.
- `Shelter Filaments`
  - Toggle.
  - Moves Abandoned Humans back into city population by lacing abandoned buildings with nanobot-maintained shelter infrastructure.
- `Infrastructure Filaments`
  - Toggle.
  - Spends Insight and Medical-Grade Nanobots to siphon Wealth, Neodymium, and Scandium from city transport flow.
- `Architectural Weave`
  - Toggle.
  - Invests Insight into citywide nanobot substrate levels; each completed level grants +1 Computing Host and +1 Computing Client.
- `Controlled Bloom`
  - Toggle.
  - Costs 400 Insight, 1 Compassion, and Medical-Grade Nanobots per turn.
  - Hostile infantry and mechs that move have a chance to gain Grey Goo. Vehicles are unaffected.

### Integrated Humans

Integrated Humans are people carrying nanobot-mediated Anthroneuroweave interfaces. They are not counted as Mind Farm occupants or Torment victims. The design intent is to avoid double-dipping while giving Integration its own population, Neural Expansion, and science identity.

Integrated Humans currently:

- Generate Insight.
- Contribute Integration Neural Expansion.
- Increase city immigration pressure.
- Support Shared Triage through the logic of medical nanobots shed into damaged systems.
- Can participate in a public-health pact that upgrades Abandoned Humans if the player accepts and funds it.

### Voluntary Integration

The voluntary branch uses Nanobot Upgrade Hubs and Worker Sledges to offer Integration directly. It spreads more slowly and begins with a lower population ceiling, but produces cleaner and more useful Insight per person.

Consent Cascade currently lets voluntary Integration go deeper into the city while it is maintained.

### Coercive Integration

The coercive branch uses Nanite Wind Generators to disperse safeguarded nanobots through their radius. It spreads faster and can reach broader coverage, producing stronger raw Neural Expansion but weaker Insight per person.

If TMI previously gave AGI researchers direct access to its mind, the coercive branch is blocked with flavor implying that TMI is being steered away from a bad idea for reasons that are not deja vu.

### Medical-Grade Nanobots

Medical-Grade Nanobots are produced by the Medical-Grade Nanobot Replicator. They are currently used by Shared Triage and are intended to become an increasingly important cost/pressure point as the nanotech side of the mod grows.

### Nanobot Rounds And Grey Goo

Nanobot Rounds are an android augment, not a weapon replacement. They increase Combat Power and Intimidation and make physical attacks apply Grey Goo.

Grey Goo stacks and deals delayed damage. Each turn there is currently a 35% chance for one stack to fall off, and stacks can spread to nearby enemies when a target dies. The current implementation is intentionally useful, but not final balance.

## Tentative Roadmap

These are current design directions, not promises that every item will land exactly this way.

- Create a player-facing text audit artifact so the narrative can be reviewed and rewritten in bulk.
- Rewrite and deepen project text after the current pacing pass, especially around the difficulty of nanobot engineering.
- Rework Espia Telecom into an earlier blackmail/investigation branch after Field Nanotech Harvest.
- Add a controlled Grey Goo scare during the nanobot research chain.
- Expand the Insight menu with more meaningful purchases, toggles, and one-time Integration upgrades.
- Add human-interaction contemplations that produce resources like Compassion, Wisdom, Creativity, and related VR resources.
- Investigate `Target Consensus`, likely as a future upgrade to Designate Coordinated Attack that adds Grey Goo through a more careful artillery/Harmony hook.
- Investigate `Compliance Lattice`, likely through replacement Nanite Wind Generator jobs with `JobRequiredDeterrence`.
- Revisit Nanobot Upgrade Hub economics and possibly move the advanced voluntary branch away from robotic upkeep toward organic/nanobot upkeep.
- Audit vanilla contemplations for obvious Integration successor content, including possible uplifted-race content later.
- Design a T3-scale Integration goal.
- Design when Vorsiber learns about the nanotech, what they demand, and how TMI can answer.

## Development Status

Organic Integration is playable end-to-end through the current Integration and Insight foundation, but it is still experimental. Numbers, pacing, text, and even some system shapes are expected to change.

Known issue tracking lives on GitHub:

https://github.com/SeanTrig/organic-integration/issues

Feedback is appreciated in any useful form:

- Playtest reports.
- Balance concerns.
- Loader errors or save/load problems.
- UI weirdness.
- Writing notes.
- Code review.
- Ideas for successor content that seems obvious once nanobots exist.

## Repository Notes

- `OrganicIntegration/` contains the mod package.
- `OrganicIntegration/_Code/` contains the C# source for custom mod logic.
- `OrganicIntegration/ModdableLogicDLLs/OrganicIntegration.dll` is the built DLL used by the game.
- `ISSUES.md` is a quick pointer to GitHub Issues.

## Credits

Created by Firesworn and Codex.

*Heart of the Machine* is by Arcen Games. This mod is unofficial and not affiliated with Arcen Games.
