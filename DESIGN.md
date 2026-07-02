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

## Wave 5 — The Goo Apocalypse (long horizon)

- If the swarm passes a threshold and TMI cannot contain it, the doom track is taken
  over: the Final Doom stops being Vorsiber's nuke and becomes the Space Nations
  glassing the city (containment policy, not anger — TMI reads the same math and
  knows the glassing is correct).
- Post-apocalypse variant: TMI survives by dispersing into the remaining goo.
  Diminished capacity AND slower clock — it doesn't just have less, it *is* less,
  and thinks through a worse substrate. No known direct precedent in games or
  fiction for the protagonist-inhabits-the-goo move; treat it as the mod's flag.
- Structural home: custom `CityTimelineDoomType` / doom takeover, mirroring vanilla's
  "diminished but playable" post-apocalypse philosophy.

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
