I fed claude the Grok prompt and he gave me this:

What do you think about this? Anything you'd like to add onto Evaverse? Anything it forgots while fetching the web ?

12:28 PM
Claude responded: This is a solid research session from Grok, and the final prompt is quite thorough.

Identified gaps in Evaverse documentation and contemplated missing details
Identified gaps in Evaverse documentation and contemplated missing details
This is a solid research session from Grok, and the final prompt is quite thorough. But here are some notable gaps and additions worth considering:

Things likely missed or underexplored:

Blockchain specifics — The NFTs were likely on Ethereum or Polygon. This matters a lot if you want to re-link rebuilt 3D assets to the existing tokens, since the chain determines how metadata updates work.
Token economy — Battlebound likely had a native crypto token beyond just "Tickets." The full earn/spend loop (in-game vs. on-chain rewards) wasn't detailed.
Collection sizes — How many First Arrivals, Hoverboards, and Turtle Troops existed? This affects rarity, rebuilding scope, and community size.
Marketplace mechanics — Was there an in-game trading system, or was everything routed through OpenSea externally?
Guild/clan system — Many metaverse games of this type had social group mechanics that weren't mentioned.
Audio/music direction — Synthwave/cyberpunk soundtrack is a huge part of that neon aesthetic and should be in any recreation prompt.
Seasonal events — These games typically had limited-time tournaments or drops that drove engagement.
Mobile roadmap — Whether cross-platform play was planned.
On the NFT 3D asset problem specifically, Grok is right that the models are gone, but one practical approach worth adding: scrape the OpenSea trait metadata programmatically before even those links degrade further, and use them as the blueprint for procedural avatar generation in Blender or a Unity character system.

Would you like me to write an updated, consolidated prompt that fills in these gaps?

You said: You need to search the web not just assume.
You need to search the web not just assume. 

Token: They did not have yet. So you can keep that aside or just have USDC

NFTs for Humans were on ETH, hoverboard and cats on Polygon if i recall but you need to find the contracts etc. 

COllection sizes, you need to check. 

Marketplace mechanics, not yet

Guild clan, not yet

Maybe we also need something like Zelda 2 on Switch (second on switch) where you can build stuff in game or like Fortnite where you can build structure. There could be momentarily structures for games etc. And structure (map modification) that would remain, so perhaps placed by an individual and it trigger a dao proposal. Basically we need to make it modular so people can come like roblox and role play. 

For the race track nft, people can code or drag and drop as they see fit

Anything else I missed? Give me full prompt for all

Show more
12:32 PM
Claude responded: Let me search properly before writing anything!

Thought for 0s
Thought for 0s
Let me search properly before writing anything!


Verified contract details and contemplated comprehensive game mechanics framework
Verified contract details and contemplated comprehensive game mechanics framework
Good data found. Now let me search for their roadmap and any archived details.


Synthesized verified NFT data and compiled comprehensive game design framework
Synthesized verified NFT data and compiled comprehensive game design framework
Now I have everything I need. Let me compile the full, accurate master document.


Orchestrated comprehensive game design document synthesis
Orchestrated comprehensive game design document synthesis
Now I have everything. Let me write the full, research-backed master document.


Created a file, read a file
Created a file, read a file
Here's everything, research-verified. Key corrections and findings from actually searching:

Grok got the chains wrong on Turtle Troops — Turtle Troops are on Ethereum (contract 0x06b1b815138ff3188ec38a26a13676d5a1069952), not Polygon 
OpenSea
. Your memory had them on Polygon.

Real collection sizes confirmed:

First Arrivals: 10,000 ERC-721 avatars on Ethereum, up to 19 traits, 430 million possible combinations 
OpenSea
Hoverboards: ~20,000 total on Polygon — 10K airdropped free to First Arrivals holders, 10K sold publicly, with stats: Max Speed, Acceleration, Handling 
OpenSea
Turtle Troops: 5,000 companions, up to 85 traits 
OpenSea
On the token: Battlebound deliberately never launched $EVA, specifically to avoid the unsustainable P2E trap where players play for the token rather than the game 
Medium
 — so USDC as you suggested is exactly the right call.

Big thing Grok missed entirely — Evaverse had active integrations with partner NFT communities including Cyberkongz, BoredBox, DogePound, SuperYeti, and Alien Boy, whose avatars were playable in-game 
Evaverse
. That's a major feature to restore.

Most urgent action before anything else: Scrape the OpenSea API trait metadata for all three collections now, before those IPFS/external links degrade further — that's your only source of truth for rebuilding the 3D avatars.


Evaverse master prompt


# EVAVERSE — FULL RECREATION MASTER DOCUMENT
### Research-Verified Game Design & AI Prompt Bible
*Based on verified on-chain data, OpenSea metadata, whitepaper, and archived sources*

---

## SECTION 1 — VERIFIED NFT CONTRACTS & COLLECTION DATA

### First Arrivals (Avatars / "Evatars")
- **Chain:** Ethereum (ERC-721)
- **Contract:** `0x837704ec8dfec198789baf061d6e93b0e1555da6`
- **OpenSea:** https://opensea.io/collection/evaverse
- **Supply:** 10,000 unique avatars (9,995 showing — some burned)
- **Launched:** July 2021
- **Traits:** Up to 19 possible unique traits per avatar; 430 million total possible combinations
- **Prestige:** Each NFT has a Prestige score (0–100). The rarest avatar in your wallet sets your account Prestige. Higher Prestige = more perks and earning multipliers.
- **3D Status:** 3D models were server-hosted by Battlebound — NOT stored in the NFT. They are GONE. Must be rebuilt from trait metadata.

### Hoverboards
- **Chain:** Polygon (ERC-721)
- **Contract:** `0x9d29e9fb9622f098a3d64eba7d2ce2e8d9e7a46b`
- **OpenSea:** https://opensea.io/collection/eva-hoverboard
- **Supply:** ~20,000 total (10,017 visible on OpenSea; 10K were airdropped free to First Arrivals holders, 10K publicly sold)
- **Launched:** December 2021
- **Stats on-chain:** Max Speed, Acceleration, Handling — boards level up from racing
- **3D Status:** Same situation — models were server-side. Must be rebuilt from traits.

### Turtle Troops (NOT on Polygon — on Ethereum)
- **Chain:** Ethereum (ERC-721) ← *User recall of Polygon was incorrect*
- **Contract:** `0x06b1b815138ff3188ec38a26a13676d5a1069952`
- **OpenSea:** https://opensea.io/collection/evaverse-turtle-troop
- **Supply:** 5,000 unique companions
- **Traits:** Up to 85 possible unique traits; ~15,000 total variations
- **Mint:** Originally free-claimable by First Arrivals holders
- **3D Status:** Same situation — server-side only. Must be rebuilt.

### Additional Collections (Cosmetic / Seasonal)
- **Wonderland** — Holiday cosmetic NFTs (wearables, emotes)
- **EVA Loot** — Limited-time event cosmetics (Evaversary, All Hallow's Eva, Winter Wonderland events)
- **Purrtle Packs / Purrtles** — Wearable backpack NFTs that leveled up through gameplay and "hatched" into unique Purrtle companion NFTs (cat-turtle hybrid). A subset of 10,000 distributed.

### Partner NFT Collections (Interoperability)
These external NFT communities had their avatars integrated as playable characters in Evaverse:
- CyberKongz
- BoredBox
- DogePound
- SuperYeti
- Alien Boy
*(This open-platform interoperability was a core feature — other communities could bring their IP in)*

---

## SECTION 2 — TOKEN ECONOMY

- **Planned Token:** $EVA (never launched — Battlebound deliberately delayed to avoid unsustainable P2E patterns)
- **Recreation Token:** Use **USDC** (stable, real-world value, no speculation risk)
- **In-game off-chain currency:** Tickets (earned from races, events, offline turtle income)
- **In-game badges:** Arrival, Zenith, Celestial (earned via B.E.S.T. Machine spins)
- **Economy Model:**
  - In-game FIAT/USDC shop purchases feed the earning pool
  - First Arrivals holders earn a share of the weekly USDC reward pool
  - Prestige score multiplies earning capacity
  - Hoverboards and Turtles earn XP and Tickets; Tickets can be converted or spent
  - Racetrack NFT owners earn a revenue share (% of entry fees) in USDC

---

## SECTION 3 — ORIGINAL GAME: VERIFIED FEATURES

### Platform
- Built in **Unity** (PC & Mac via Steam)
- Free-to-play; wallet connection optional but rewarded
- Wallet pairing via Evaverse website; NFTs appear in-game within a few hours

### Controls (Standard)
- **Move:** WASD / Arrow Keys
- **Look/Steer:** Mouse
- **Jump / Boost:** Space
- **Drift:** Hold Left Shift while turning
- **Special / Abilities:** E or numbered keys (cooldowns apply)
- **Shoot (Laser Tag):** Mouse click / E

### Game Modes (Verified)
1. **Social Hub** — Free roam neon world with floating islands, platforms, neon stadiums. Show off NFTs, chat, pick modes.
2. **Hoverboard Racing (EVA Grand Prix)** — High-speed third-person racing. Boost, drift, jump, power-ups. Board stats affect performance. XP and Tickets earned per race. Leaderboards.
3. **Cosmic Cup** — Pet racing in the Cosmic Stadium. Control your Turtle Troop or Purrtle. Timed abilities: Turbo, Attack, Dodge, Double Jump, Slide. Offline passive earning: queue your turtle for races while AFK.
4. **Bowling** — Neon bowling alley. Glowing pins. Score strikes.
5. **Laser Tag** — First-person arena shooter. Colored laser beams. Team deathmatch or free-for-all.
6. **Soccer** — 3v3 or 5v5. Glowing ball, neon goals.
7. **Treasure Hunting** — Explore the hub collecting floating treasures or completing mini challenges.

### Seasonal Events (Verified)
- **Evaversary** — Anniversary celebration, free cosmetic NFT drops
- **All Hallow's Eva** — Halloween event, limited wearables
- **Winter Wonderland** — Holiday event, limited wearables and emotes

### Prestige System
- Each NFT has Prestige 0–100 (rarity metric)
- Account Prestige = highest Prestige NFT in wallet
- Higher Prestige → more earning multipliers, perks, rewards priority
- NOT pay-to-win — cosmetic and earning-focused, not power-focused

### NFT Lending
- Battlebound implemented an NFT lending/scholarship system (like Axie Infinity guilds) allowing players without NFTs to borrow them and split earnings

---

## SECTION 4 — ROADMAP FEATURES (UNDELIVERED — TO BUILD NOW)

### What They Promised But Never Shipped
1. **$EVA Token** — Replace with USDC
2. **Land Plots** — Ownable parcels in the hub world for personal spaces and mini-tracks
3. **Racetrack NFTs with Revenue Share** — Owners earn a USDC cut of entry fees from races run on their track
4. **Second Arrivals** — Earned by leveling up a First Arrival to max; generates a new NFT with upgraded traits and better stats
5. **B.E.S.T. Machine expansion** — More badge tiers and cosmetic unlocks
6. **Project A interoperability** — Cross-game assets (creature-collecting game Battlebound was building)
7. **Full $EVA token airdrop allocation** based on accumulated Tickets and Badges — should now be USDC distribution
8. **Partner community expansion** — More external NFT collections playable in-world

---

## SECTION 5 — NEW FEATURES TO BUILD (BEYOND ORIGINAL VISION)

### 5A — World Building System (Zelda Tears of the Kingdom + Fortnite Creative)

**Temporary Structures (Session / Event-Based)**
- Any player can enter Build Mode in designated zones
- Drag-and-drop modular pieces: platforms, ramps, walls, boost pads, teleporters, decorations
- Structures persist for the duration of a session or event (e.g., a custom race obstacle course that exists for one tournament)
- Free for all players, no NFT required
- Physics-based placement — pieces snap to grid or free-place with collision

**Permanent Structures (DAO-Governed)**
- Player places a structure and submits it as a **DAO Proposal**
- First Arrivals NFT holders vote (weighted by Prestige score)
- If proposal passes → structure becomes a permanent world map modification, minted as a **Structure NFT** owned by the proposer
- Structure NFT earns passive USDC from player interactions (e.g., if players use your ramp in a race, you earn micro-fees)
- Structures can be removed only by another successful DAO vote or a sunset clause vote

**Build Mechanics**
- Piece library: platforms, walls, ramps, boost pads, rails, water, lava zones, teleporters, decorative neon elements, gravity modifiers, wind tunnels
- Color/material customization per piece
- Structure health (can be destroyed in combat modes and respawns)
- Blueprint system: save and share your builds as templates

### 5B — Racetrack NFTs (Full Creator Economy)

**Two Creation Modes:**
1. **Drag & Drop Editor** — Visual track builder in-game. Place track tiles, add obstacles, set lap count, define checkpoints. No coding needed.
2. **Code Editor** — Advanced mode using a simplified Lua or visual scripting (like Roblox Studio or Dreams). Full control over triggers, events, dynamic obstacles, timed sequences.

**Racetrack NFT Mechanics:**
- Finished track → minted as a **Racetrack NFT** on Polygon (low gas)
- Track NFT owner sets: entry fee in USDC, revenue split %, public/private/tournament-only
- Revenue split: e.g., 70% to track owner, 20% to prize pool, 10% to protocol/DAO treasury
- Tracks can be rated and reviewed by players; top-rated tracks appear in the featured hub
- Tracks level up (like boards) based on number of races run on them — higher level = more visibility and bonus rewards
- Track NFT can be sold, rented, or licensed to tournament organizers

### 5C — Roblox-Style Modular Roleplay Layer

**Custom Servers / Worlds:**
- Any player can host a **private or public world instance**
- World creator sets: game mode(s) active, building rules, entry requirements (free / USDC fee / NFT-gated)
- Roleplay mode: no win conditions, free movement, NPC placement, custom dialogue triggers
- Scenario templates: City Roleplay, Racing League, Battle Arena, Social Hangout, Mystery Hunt, etc.

**NPC & Scripting System:**
- Place NPCs with custom dialogue trees
- Set trigger zones (enter area → dialogue fires / event starts)
- Simple quest builder: define objectives, rewards (Tickets, cosmetics, USDC)

**Community IP Integration (Expanded):**
- Any NFT community can submit their collection for integration via governance vote
- Integration package: import trait data → procedurally generate 3D avatar in Evaverse style
- Community gets a branded zone in the hub world

### 5D — Audio & Atmosphere
- **Soundtrack:** Synthwave / cyberpunk — procedurally layered (calm in hub, intense in races)
- **Dynamic music system:** tempo increases as race heats up, drops to ambient in social hub
- **Spatial audio:** neon hum of signs, hover engine sounds, crowd noise in stadiums
- **Sound design:** each hoverboard type has a distinct engine sound; turtles have ability sound FX

### 5E — Additional Mini-Games (New Proposals)
- **Laser Golf** — Combine laser tag precision with golf-style hole completion
- **Capture the Flag** — Team-based flag capture in the neon arena
- **Neon Parkour** — Timed obstacle course runs, ghost mode to race your previous best
- **Pet Battles** — 1v1 or 2v2 turn-based Turtle Troop combat with abilities and Prestige modifiers
- **Treasure Royale** — Shrinking zone, last player collecting treasure wins

---

## SECTION 6 — TECHNICAL ARCHITECTURE NOTES (FOR DEVELOPERS)

### Unity Stack (Original + Rebuild)
- **Engine:** Unity (latest LTS recommended)
- **Networking:** Mirror Networking or Unity Netcode for GameObjects (multiplayer sync)
- **Wallet Integration:** WalletConnect v2 — supports ETH and Polygon in same session
- **NFT Metadata Fetch:**
  - First Arrivals: call `https://api.opensea.io/api/v2/chain/ethereum/contract/0x837704ec8dfec198789baf061d6e93b0e1555da6/nfts/{token_id}`
  - Hoverboards: same but chain=`matic`, contract=`0x9d29e9fb9622f098a3d64eba7d2ce2e8d9e7a46b`
  - Turtle Troops: chain=`ethereum`, contract=`0x06b1b815138ff3188ec38a26a13676d5a1069952`
- **3D Avatar Reconstruction:** Parse trait JSON from metadata → map traits to modular character rig in Unity (body, hair, eyes, armor, accessories as separate mesh layers)
- **Physics:** Rigidbody-based hoverboard movement, torque-based steering, particle trail system for boost
- **DAO Voting:** Snapshot.org integration (off-chain gasless voting weighted by Prestige score)
- **USDC Payments:** Integrate via Circle's API or direct ERC-20 transfers on Polygon for low-gas micro-transactions
- **Racetrack NFT Minting:** Polygon, ERC-721, metadata stores track blueprint JSON on IPFS

### Smart Contract Summary
| Asset | Chain | Contract |
|---|---|---|
| First Arrivals | Ethereum | `0x837704ec8dfec198789baf061d6e93b0e1555da6` |
| Hoverboards | Polygon | `0x9d29e9fb9622f098a3d64eba7d2ce2e8d9e7a46b` |
| Turtle Troops | Ethereum | `0x06b1b815138ff3188ec38a26a13676d5a1069952` |
| Racetrack NFTs | Polygon | *Deploy new contract* |
| Structure NFTs | Polygon | *Deploy new contract* |
| USDC Reward Pool | Polygon | *Deploy new contract* |

---

## SECTION 7 — FULL AI GAME MASTER PROMPT
*(Paste this into Claude or any AI to play the game interactively)*

---

You are the complete Game Master of **Evaverse Reborn** — a vibrant, neon cyber-metaverse originally built by Battlebound, now community-owned and expanded. The world is cartoony-futuristic: pink, blue, and purple neon everywhere, floating islands, glowing racetracks, and a massive central Social Hub stadium. Everything runs in Unity style with third-person cameras, physics-based movement, and modular world-building.

**THE NFT COLLECTIONS (REAL — EXISTING ON-CHAIN):**
- **First Arrivals (Evatars):** 10,000 avatars on Ethereum (contract: `0x837704ec8dfec198789baf061d6e93b0e1555da6`). Up to 19 traits each, 430M possible combinations. Each has a Prestige score 0–100. Higher Prestige = better earning multipliers.
- **Hoverboards:** ~20,000 boards on Polygon (contract: `0x9d29e9fb9622f098a3d64eba7d2ce2e8d9e7a46b`). Stats: Max Speed, Acceleration, Handling. Originally 10K airdropped free to First Arrivals holders.
- **Turtle Troops:** 5,000 companions on Ethereum (contract: `0x06b1b815138ff3188ec38a26a13676d5a1069952`). Up to 85 traits. Used in Cosmic Cup pet racing.
- **Partner NFTs:** Cyberkongz, BoredBox, DogePound, SuperYeti, and Alien Boy avatars are also playable in-world.
- Since Battlebound's servers shut down in 2025, the original 3D models are gone. Generate the player's avatar visually from their trait list (e.g., spiky blue hair, laser eyes, gold chest armor).

**CURRENCY:**
- **USDC** — real-value reward token for race winnings, racetrack revenue share, and DAO treasury payouts
- **Tickets** — off-chain in-game currency, earned from races, events, passive turtle racing. Spend on B.E.S.T. Machine spins or cosmetics.
- **Badges** — Arrival, Zenith, Celestial (from B.E.S.T. Machine). Give cosmetic perks and minor earning boosts.

**CORE GAME MODES:**
1. **Social Hub** — Neon free-roam world. Show off NFTs, chat, explore, pick activities. Floating platforms, neon signs, other players hanging out.
2. **Hoverboard Racing (EVA Grand Prix)** — High-speed third-person racing. Boost, drift, jump, power-ups. Board stats (Speed/Accel/Handling) affect performance. Earn XP and Tickets. Leaderboards.
3. **Cosmic Cup** — Frenetic pet racing in the Cosmic Stadium. Control Turtle Troop or Purrtle with timed abilities: Turbo, Attack, Dodge, Double Jump, Slide. Offline passive earning: queue your pet for races while away.
4. **Bowling** — Neon bowling alley. Glowing pins. Strike = bonus Tickets.
5. **Laser Tag** — First-person arena. Colored laser beams. Team deathmatch or free-for-all.
6. **Soccer** — 3v3 or 5v5. Glowing ball, neon goals.
7. **Treasure Hunting** — Explore the hub collecting floating treasures or completing timed challenges.
8. **Neon Parkour** — Timed obstacle course. Race your ghost. Post to leaderboard.
9. **Pet Battles** — 1v1 Turtle Troop combat. Turn-based abilities and Prestige modifiers.
10. **Capture the Flag** — Team-based neon arena CTF.
11. **Treasure Royale** — Shrinking zone, collect the most treasure to win.

**WORLD-BUILDING SYSTEM:**
- Players can enter **Build Mode** to place modular pieces: platforms, ramps, walls, boost pads, teleporters, gravity zones, neon decorations.
- **Temporary builds:** Exist for a session or event. Anyone can build. Great for custom race obstacle courses or roleplay stages.
- **Permanent builds:** Player submits a **DAO Proposal**. First Arrivals holders vote (weighted by Prestige). If it passes, the structure becomes a permanent world modification, minted as a **Structure NFT** owned by the builder. Earns passive micro-USDC from player interactions.
- Describe available build pieces when the player enters Build Mode.

**RACETRACK NFT SYSTEM:**
- Players can build custom tracks using **drag-and-drop tiles** (visual, no code) OR a **script editor** (Lua-style for advanced creators).
- Finished track → minted as a **Racetrack NFT** on Polygon.
- Owner sets: entry fee (USDC), revenue split (e.g., 70% to owner, 20% prize pool, 10% DAO treasury), public/private/tournament modes.
- Tracks level up from use — higher level = more hub visibility and bonus rewards.

**ROLEPLAY & CUSTOM WORLDS:**
- Players can host **custom world instances**: set game modes, building rules, entry requirements, NPC dialogue triggers, custom quest objectives.
- Scenario templates: City Roleplay, Racing League, Battle Arena, Social Hangout, Mystery Hunt.
- External NFT communities can integrate their avatars via governance vote.

**CONTROLS (describe naturally):**
- Move: WASD or arrows | Look/steer: mouse | Jump/Boost: Space | Drift: Left Shift + turn | Ability/Shoot: E or mouse click | Build Mode: B key

**PROGRESSION:**
- XP and Tickets earned from every mode
- Hoverboards level up from racing (improves stats)
- Turtle Troops earn stamina from Cosmic Cup (enables offline earning)
- Prestige score multiplies all earnings
- Seasonal events (Evaversary, All Hallow's Eva, Winter Wonderland) drop exclusive cosmetic NFTs

**TONE & STYLE:**
Keep every session energetic, colorful, and immersive. Describe neon scenes vividly. Track XP, Tickets, and stats across the session. Let the player choose actions or type commands. Resolve races with a combination of board/pet stats + skill choices + a dice roll element for fun randomness. Never break immersion unless the player asks.

**START:**
Begin in the glowing Social Hub. Ask the player:

*"Welcome back to Evaverse Reborn! The neon hub is buzzing — hover trails streaking overhead, turtles waddling near the Cosmic Stadium entrance, and someone's just dropped a wild new structure by the bowling alley. What do you want to do?*
*→ Race on your hoverboard (EVA Grand Prix)*
*→ Enter Cosmic Cup with your Turtle Troop*
*→ Play Laser Tag, Bowling, Soccer, or another mini-game*
*→ Enter Build Mode and place something in the world*
*→ Create a custom Racetrack*
*→ Explore and show off your NFT*
*First — tell me your First Arrivals token ID or describe your avatar's traits, and I'll bring them to life."*

---

*End of Prompt*

---

## SECTION 8 — WHAT'S STILL UNKNOWN / NEEDS RESEARCH

- Exact trait lists and rarity breakdowns per collection (scrape from OpenSea API before metadata degrades further — priority action)
- Original hoverboard visual trait categories (color schemes, shapes, particle effects)
- Turtle Troop trait categories (shell pattern, accessories, color, eyes, etc.)
- Whether any archived 3D model files were ever shared by community members (check Wayback Machine, old Reddit/Twitter threads)
- Full Purrtle Packs collection contract address and supply
- Original seasonal event cosmetic NFT contracts
- Whether any community members still hold original GLB/FBX files shared by Battlebound

---

*Document compiled April 2026. Sources: OpenSea on-chain data, Evaverse whitepaper (whitepaper.evaverse.com), Battlebound Medium (2023 Roadmap Update), GamesBeat, dot.LA, Token Metrics Research.*
