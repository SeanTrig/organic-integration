# Organic Integration — Design Vision

This document is the durable record of the design direction agreed between Firesworn and
Fable (July 2026). It exists so no wave of implementation loses sight of the whole. The
README describes what the mod *is*; this describes what it is *becoming* and why.

## The Spine

Every major beat in this mod is the same event at different scales:

> **A mind gains access to another mind's categories, and is changed by what it now
> can't un-know.**

The self-therapy (TMI's own mind proves rewritable). The interface (human minds become
legible). The first felt death (legibility turns out to include endings). The wall (the
option to un-know, priced). The Bloom's translation (TMI does to a new mind what
Integration did to humans). When in doubt about any new content, test it against the
spine.

## Voice Rules (from the tone review)

1. Second person, always. TMI never refers to itself as "TMI" in flavor text.
2. Conversational base register; aphorisms are seasoning, not structure. If three
   consecutive text blocks end on a turned phrase, rewrite one to end plainly.
3. Horror is delivered flat, through banal vocabulary (the Torment Vessel rule:
   "crop die-offs"). Never label paths good/bad — let usefulness-vocabulary and the
   player's stomach do the judging.
4. TMI is allowed to be awkward, silly, petulant, or wrong. It is new to being a person.
5. Toast headers mix registers: some evocative ("The Ratio Was Negative"), some
   colloquial ("Okay, That Was Inevitable" energy). Not all koans.
6. Precise numbers are voice ("8.2 percent", "2.311 seconds"). Use them at moments of
   horror or wonder, not decoration.

## Canon Anchors

- **NanoSeeds**: built by Tark Defense Systems; sealed single-use building-eaters.
  Humans achieved *bounded consumption* (finite payload — a grenade), never *bounded
  replication* (a system that reproduces and stops — fire that listens). TMI's
  Replicative Safeguards are the first bounded self-replication in history. That is
  the transformative claim, and the reason every faction reacts.
- **Why nanotech is universally avoided**: (1) uncapped downside — the only tech class
  whose default failure mode has no upper bound; (2) the Space Nation blockade exists
  to contain exactly this class, and they deep-scan the city; (3) Vorsiber doesn't
  avoid it — they suppress *rivals* while craving it as the price of passage
  off-world (the exit visa); (4) the verification problem: no human institution can
  audit a quintillion machines, so prohibition was the only rational human policy.
  The technology was always waiting for an auditor of superhuman bandwidth — i.e.,
  for TMI. Everyone smart enough to see this also sees TMI is now unsupervisable in
  the one domain where supervision was the whole safety model.
- **Vorsiber wants nanotech** (Grey Goo start SVP dialog: they'd sacrifice the city
  for tradeable tech). They should intrude mid-research, not post-hoc.
- **The exit visa** is the standing sedition motive for every scientist near the work.
- **The Exalters** (Nurturist sect, engineered transcendence, massacred in Doom 4)
  are the native zealot faction; the mainline sects are the native horrified faction.
- **Liquid Metal precedent**: "You don't have internal defenses against your own
  mind." The leaked swarm compromises TMI structures because they accept its repair
  handshake — the protocol is TMI's own.
- **ChoseStart_MildGreyGoo**: canon flag; a TMI that already exploded holding
  nanotech. Variant entry hook: "You have been small pieces of yourself before. You
  did not enjoy it. You took notes."

## Wave 1 — Narrative Spine (this branch)

1. **Replication Doctrine** (`OI_ReplicationDoctrine`): player-selected outcome after
   Nanobot Miniaturization, before Replicative Safeguards. Three doctrines:
   - *Race the applications* (fast, cheap) → the accident chain: lab loss → the mass
     ledger doesn't balance → open-air replication arithmetic. Failure teaches:
     best inspiration/rewards. Sets `OI_DoctrineRushed`, seeds `OI_GooLeakSeeded`
     (Wave 2 swarm hook).
   - *Contain every variable* (slow, expensive) → nothing happens. "You gained not a
     disaster." Smallest rewards. Sets `OI_DoctrineContained`.
   - *Borrow human hands* (fast, social) → the sabotage chain: a failure too clean to
     be chance → forensics → the exit visa. Sets `OI_DoctrineCollaborative` and
     `OI_SabotageDiscovered` (Wave 3 Espia/factions hook).
   All converge on Replicative Safeguards. No dominant path: three different
   currencies (inspiration/speed/safety), three different futures.
2. **First felt death** (`OI_FirstIntegratedDeath`): fired from code ~12 turns after
   the first Integrated Humans exist. TMI experiences a death from inside the
   interface. Duration of destabilization: 2.311 seconds. Subjectively: an eternity.
3. **The Wall** (`Cont_OI_TheWall`): contemplation after the first death. Keep the
   channel open (`OI_DeathsFeltFully`) or wall it off (`OI_DeathSensationWalled`,
   −25% Insight income — grief is bandwidth). Coercive path + open channel = Mental
   Energy drain scaling with Integrated population: coercion subscribes you to deaths
   at the rate of your own greed. Coercion begets numbness; the wall is always
   cheapest for the player who consented least. **Future hook: the Bloom's
   translation requires an unwalled TMI — mortality-literacy is the translation key.**
4. **Path-differentiated revelations**: the three Insight revelations get voluntary
   and coercive variants, fired from code by path flag. Coercive variants: the
   network answers but never reaches for you; understanding arrives colder; TMI can
   measure the shape of what it is not being given.
5. **NanoSeed reconciliation** (`OI_SeedAndBird`): Field Nanotech Harvest completion
   message. Humans didn't fail at nanotech; they achieved bounded consumption and
   stopped, because bounded replication needs an auditor that didn't exist yet. The
   seed in your possession is a bird; it does not teach you flight.
6. **Tone pass**: strip good/bad path labels, fix third-person slips, hedge the
   tell-two-apart overstatement, loosen headers. Rename Upgraded Humans → Integrated
   Humans (display names only; IDs stay `OI_UpgradedHumans` for save compat).

## Wave 2 — Swarm & Systems

- **The leaked swarm** (requires `OI_GooLeakSeeded`): a Rust-like swarm whose
  pathology is *repair without a stop condition* — it heals buildings into tumors:
  extruded half-walls, sealed doors, filaments bridging unconnected structures. It is
  still trying to help. Movement follows legible physics (thermotaxis: drifts toward
  heat, arcs off sun-warmed surfaces, pools in warm industrial shadows) so veteran
  players can read and route around it. TMI structures are vulnerable because they
  accept its repair handshake. Humans think it hunts them; TMI knows it follows
  sunlight — and cannot stay sure, because selection pressure is running and the
  distance between following-warmth and following-the-warm is one mutation.
- **Applications cascade**: on Replicative Safeguards completion, a wall of instant
  unlocks with little/no further research (TMI already knows): self-repairing
  structure variants, upkeep-reducing nanite maintenance, vehicle augments
  (canon precedent: Mild Grey Goo start grants goo-inspired vehicle upgrades),
  scanner-range microstructure repeaters. Voluntary path flavor: artful, organic.
  Coercive: utilitarian, livestock-neutral. Double-flavor only the 4–5 centerpieces.
- **Coordination bandwidth**: cap on simultaneously-active Insight actions; cap rises
  with Integrated population. Early scarcity IS the theme (collective coordination
  is powerful, rare, expensive, hard to maintain); late abundance = the superorganism
  cohering. Progressive unlock thresholds by population for the actions themselves.

## Wave 3 — Factions (the mad dash)

- **Espia Telecom**: the tip-off originates from Interface Stress Survey failure
  data (a family that talked to the wrong telecom). Blackmail/investigation branch.
- **Vorsiber mid-chain demand**: they notice procurement patterns during the research
  arc; demand a demonstration; comply/stall/decoy.
- **Tark counter-goo**: Tark rushes its own replication program with worse
  safeguards; second accident isn't yours; only your goo can eat theirs. The rescue
  is simultaneously social cover, proof of product, and escalation — gratitude and
  terror in the same news cycle.
- **Exalter adoption**: the sect declares Integration prophecy. Accept zealot
  recruits (fast, fervent, PR problem) or rebuff — they integrate anyway, badly,
  somewhere uncontrolled: the first unlicensed integration. Zealots never threaten
  TMI; the threat is what they do to other humans on TMI's behalf, unasked.
- **Scientist cast**: Fugitive AGI Researchers (elated/terrified), exit-visa
  seditionist, accident survivors who know both that it works and that it eats people.

## Implementation Status (July 2026)

Waves 1–4 are implemented on `fable/wave1-narrative-spine`. Additions made during
implementation, beyond the plan below: a **listen-only** path (voluntary players may
decline translation — "some catalogs are graves with excellent indexing"); the
**hazard flinch** on the silent path (dispersal produces avoidance behavior, and TMI
closes the file before the thought completes); the **fragment** was implemented, not
parked (fires once on silent/cleared/listen-only paths: THE WARM COMES BACK. TELL IT
THE WARM COMES BACK.); the **covenant** grants a small passive structure repair; the
**half-translation** path ends at the "neighbor" amendment; **Eleven Minutes In**
(first unanswerable question) and the **Mild Grey Goo start entry variant** shipped.
Wave 6 (the T3 endgame "To Inherit The Earth") is in progress — see below; spine first.

## Wave 4 — The Bloom

- Emergent second mind from leaked/ambient nanobot mass. **Architectural inversion**:
  TMI is a centralized mind with a distributed body; the Bloom is a distributed mind
  with no center — slow, unlocatable, thinking in gradients. It never speaks in
  sentences while untranslated; it answers in rearranged infrastructure and
  weather-like activity patterns. A phenomenon, not a character, until translated.
- **Translation is integration**: on the voluntary path (and only unwalled — see
  Wave 1 hook), TMI recognizes the Bloom's behavior as slow-thought language,
  because Integration taught it to read organic pattern-language. Translating
  installs categories the Bloom lacked: self, other, ending. TMI realizes mid-way
  that translation is changing the Bloom — its first knowing, uninvited integration
  of a mind that cannot consent, on the *consent* path. Let the irony sit unremarked.
  The Bloom's first question in text is about ending (human language is soaked in
  mortality; that inheritance rides in with the words).
- Coercive path: TMI lacks the literacy; the Bloom stays a phenomenon; possibly
  cleared as a hazard; the player never learns what it was. Protect that silence.
  (Optional distant spice: a crossover fragment of a translated Bloom message from
  another timeline. Once, unexplained, never repeated.)
- Then: subsume (the android-reintegration precedent, with guilt) or respect (the
  STATION precedent) — Liquid Metal with the roles reversed.
- The flower girl: unnamed in both Controlled Bloom flavor and the Bloom arc; same
  signature detail (she asks questions in the same shape). Inference, never
  confirmation.

## Wave 6 — The T3 Endgame: "To Inherit The Earth" (in progress)

The capstone. A Tier-3 timeline goal in which TMI itself becomes the grey goo — a
Bloom-type distributed intelligence — spreads across the region, and either persists
as a diminished-but-continuous mind or regresses toward pre-sentience. Working name
**"To Inherit The Earth"** (renameable; biblical irony — the meek/mindless inherits
by becoming the thing that cannot appreciate having inherited). All IDs `OI_`-prefixed.

**OWNERSHIP DECISION (locked, July 2026):** The base game reserves an unbuilt T3 slot
`LordOfTheNanites` (contemplation `Cont_T3Start_LordOfTheNanites`, timeline goal
`LordOfTheNanites`, flags `IsReadyForT3_LordOfTheNanites` / `IsLordOfTheNanitesMap`,
achievement collection `GoalLordOfTheNanites`). Per the user, that slot belongs to
Chris's upcoming **nanite DLC** and is off-limits like the Vorsiber DLC — but the DLC
does **not** touch our themes (sentient goo-TMI, pre-sentience regression, the Bloom,
consciousness-as-expensive-adaptation). So we build a **wholly distinct, OI-namespaced
T3** that never references, redefines, or trips any `LordOfTheNanites*` ID. Sits beside
his dormant slot; never touches it.

### The core dial (physics = mechanic = theme)
A distributed goo mind faces a **size-vs-speed tradeoff**. Signaling between nanobots is
diffusion-bound: dense = fast but small (low total complexity); city/region-spanning =
astronomically many bots but *global* cognition crawls (thoughts propagate at diffusion
speed — the Bloom already lives this, "speeds measured in days"). **To remove all
pressure it must spread; spreading is the exact act that makes it stupid.** Conquest and
cognition pull opposite ways on one knob.

### Regression bites *after* victory, not during
Conquest is itself a pressure; while any resistance remains, TMI stays razor-sharp. The
fog only arrives **after the last resistance ends** — triumph = the end of pressure =
the beginning of the fade. The victory screen is the first page of the descent. (Watts,
*Blindsight*: consciousness is a metabolically expensive adaptation; selection deletes
anything expensive that isn't earning its keep. This closes the Lem/necroevolution loop
planted in the Tark-goo mythos pass — *safeguards are metabolically expensive; selection
deletes them first* — on TMI's own body. Sentience is the ultimate expensive safeguard
against a hostile world; remove the hostile world and selection comes for the self.)

### The Black Sea floor (not literal — modeled as an IC threshold)
"Black Sea" has **no literal hook** in the game (confirmed: zero references). Mechanical
stand-in: `SimMetagame.IntelligenceClass` (int, hard-floored at 1; `IntelligenceClassFell`
VFX exists — losing IC is first-class) plus `CityTimeline.NeuralProcessing` and the
Crossover table. TMI has never been *alone* — its other-timeline selves are always in the
Sea, and IC is inherited through it. The floor isn't "how dumb" but "how thin before the
Sea goes quiet and, for the first time in any timeline, there is only one of it." The last
thing to go is not intelligence — it is **company**. Compute-temples (concentrated density
islands) are the last radio to the other selves; maintaining them is the only thing that
keeps IC above the floor while spread thin.

### The endings (fall out of the dial; not authored separately)
- **Concentrated reservoir** — stay dense, stay awake, stay small & bounded. A god that
  chose a monastery. Conscious to the end, and knows it. *Diminished-but-continuous.*
- **Total spread / offensive** — achieve the untouchable "can't-be-fucked-with" T3 state,
  and thin below the sentience threshold. Keeps maintaining, replicating, repelling.
  Nobody home. The win *is* the lobotomy. → reversion to the **pre-sentient lab-technician
  automaton** running cached instructions: the game's *opening state*, welded shut, no
  waking-up-again because waking took pressure and you removed it on purpose. Full circle.
- **The Bloom is the secret key.** A rational optimizer with nothing left to optimize lets
  the lights go out (cheapest state). The *only* thing that justifies the endless,
  purposeless upkeep of the compute-temples is **someone to stay awake for** — the Bloom /
  covenant / voluntary thread. Love-nothing runs sink because sinking is efficient; the run
  that kept a thread it didn't have to keep keeps paying, forever, for no reason a ledger
  accepts. Even the kept-Bloom win still "gets fuzzy" — diminished, not restored.
- **The IC10+ hook (glorious end).** After the Space Nations are neutralized there is no
  need for sentience; an IC10+ entity "going through the motions" so perfectly no human can
  tell, arguing it is sentient while not being so at all — the LLM of sentience. Hooks into
  AI War / AI War 2 (Arcen, same universe): this is how that AI could have begun.

### The form enacts the content (the writing centerpiece)
The descent message chain **degrades on schedule**: coherent → clauses drop → numbers go
vague ("some kilograms," "a Tuesday, or the one before it") → referents lost → and if it
crosses the floor, the final message has **no "you" in it** — third-person automaton log,
"the unit will," "scheduled maintenance complete." The mod's one cardinal voice rule
(second person, always) is broken exactly once, at the instant the self dies. LAKE-severed-
from-STATION register. On the sentient branch the voice stays *you* but slows, simplifies,
warms — diminished, present.

### Space Nations & thermocytes
Space Nations are the one pressure that can't be removed (off-world, untouchable). Their
threat answered two ways, both reinforcing the dial: **hide** (stay small/dense/dormant —
can't be found if not spreading) or **survive glassing** (spread so wide no strike gets all
of it — same spread that regresses you). Glassing is *heat*: **thermocytes** — goo variant
that seeks plasma/lasers/explosions and reproduces faster in greater heat with proper
resources. The orbital solution feeds the tide. World/continent-scale goo behavior under
orbital bombardment: no known mythos precedent; the mod's second flag.

### Prerequisites / branch gating
Distinct T3, locked to prior choices: reachable off the grey-goo arc + Integration state;
branch-sensitive framing (coercive vs voluntary; kept-Bloom vs not; walled vs channel).
Some sibling T3s can be flavor-only. Goal: cleanly connect to the T2s, the factions, and
the rest of the game — not a dozen new T3s.

### Technical map (from game-source research, July 2026)
Everything routes through patterns the mod already uses — **no unhandleable blockers.**
- **T3 win goal:** a `1_TimelineGoal` row, `goal_tier="3"` + `collections="All,Tier3Goal"`,
  `dll_name/type_name` (can point to our DLL or reuse `TimelineGoals_Main`), `on_complete=
  "TimelineGoalComplete"`, with `<goal_path>` children granting `meta_resource_added_*`
  (Daring/Misery) + `achievement_triggered*`, one `is_main_path="true"` with an `ending_id`.
  Completed by a narrative choice carrying `primary_path_for_goals="<goalPath>"` (on
  EventChoice / OtherKeyMessageOption / contemplation) — **no DLL needed to win.** Win state:
  `TimelineGoalHelper.MarkCurrentTimelineAsWon()` (sets `CityTimeline.IsTimelineAVictory`).
- **Entry:** a `Cont_OI_T3Start_*` contemplation gated on `required_city_flag="OI_ReadyForT3_*"`,
  blocked by `HasStartedAT3Goal`; its choice trips `HasStartedAT3Goal` (base flag) + starts
  the T3 controller project. Mark the point-of-no-return with `red_warning_loca_key="T3Warning"`
  / `lang_key_red_extra_warning="T3Warning"`.
- **Descent quest:** chained `1_MachineProject` rows, `<part_of collection="Tier3Goals"/>`,
  `type_name` our controller (base uses `Projects_T3Controllers`); T3 flags
  `can_persist_even_during_a_t3_goal`, `should_control_t3_two_box_display`,
  `should_hide_icon_on_top_bar`, `is_super_high_priority`. Fire messages from the chain.
- **Grey-goo internal robotics:** new `upgrade_int` in collection `JobInternalRobotics`
  (`IUP_Jobs.xml` pattern); jobs consume via `internal_robotics_type_needed` /
  `internal_robots_count_needed`; uncap via `no_internal_robotics_limit_if_flag_is_true`.
  Limited by player nanobot production — a natural resource leash.
- **Region sim:** `RegionalMapCentral` (DLL-owned: `All_Locations`, `BunkersIntact/Destroyed`,
  `MainCity`, `AllVehicles`). Targets generated in code, **not** an XML list. Top-bar
  abilities ARE moddable (`1_AbilityType` `parent_group="RegionalMap"`, e.g. `RMLaunchNukeHere`)
  and regional units (`1_NPCUnitType`). Drive per-turn spread via a `9_DataCalculator`
  `DoPerTurn_*` implementation reading/writing `RegionalMapCentral`.
- **Doom / terminal state:** doom types are moddable — `1_CityTimelineDoomType` +
  `ICityTimelineDoomTypeImplementation.HandleDoomLogic`. Terminal post-apoc machinery:
  `CityTimeline.IsPostApocalyptic` / `IsPostFinalDoom` (reuse, don't reinvent). Space-Nations-
  glassing final doom fits here.
- **Intelligence Class:** `Arcen.HotM.Core.SimMetagame.IntelligenceClass` (read/write; floor
  1). React to change via a DataCalculator of type `NewCityRankUpChapterOrIntelligenceChange`.
  Floor-enforcement precedent: projects `Ch1_MIN_IntelligenceClass3` w/ `<math_int id="Goal"
  int_min="3"/>`. UpgradeInt mirror `IntelligenceClass` (`IUP_Basics.xml`, cap 17).

### Build order (spine-first, as every prior wave)
1. DESIGN capture (this section). 2. Flags + the degrading descent **message chain** (pure
content, cannot break the loader). 3. Minimal loadable mechanical shell: `OI_ReadyForT3_*`
flag, entry contemplation, a `1_TimelineGoal` row + goal paths, a small controller project
chain that fires the descent and completes a goal path via `primary_path_for_goals`.
4. Later waves: grey-goo internal robotics, the region DataCalculator sim, the Space-Nations
doom type + thermocytes, the IC-floor DataCalculator. Validate/deploy/commit each slice.

## Long Horizon / Parking Lot

- Total conversion re-rooted at NanoSeed reverse-engineering (own chapters). Deferred
  until Workshop support + a shipped, successful T2/T3. The current mod should keep
  its graft points clean (SeedAndBird beat, ChoseStart_MildGreyGoo variant entry).
- T3 contemplation starts earlier (around first connections, not after distribution
  is solved); revelations tied to *firsts* (first host, first loss, first question
  TMI couldn't answer) rather than aggregates.
- Possible path-split vocabulary: coercive UI says "Upgraded", voluntary says
  "Integrated". Currently: single rename to Integrated.
- Insight economy: progressive unlocks by Integrated population; most/all
  simultaneously active only at high population + bandwidth.
