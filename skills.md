# Evaverse Skills

This file defines the main workstreams for the project. A contributor or agent should usually operate inside one skill at a time.

## 1. Core Feel

### Owns

- avatar locomotion
- hoverboard handling
- drift / boost / jump feel
- camera tuning

### Success Looks Like

- movement feels responsive with keyboard, mouse, and controller
- the hoverboard loop is fun before multiplayer or web3 is layered on

## 2. Networking And Sessions

### Owns

- host / join flow
- session lifecycle
- player spawn and despawn
- race synchronization

### Success Looks Like

- players can create and join a session reliably
- race state stays coherent without overengineering

## 3. Metadata And Ownership

### Owns

- wallet ownership reads
- NFT metadata fetches
- chain-specific adapters
- canonical trait schema

### Success Looks Like

- collection ownership is readable without infecting the gameplay code
- metadata ends up normalized into stable internal models

## 4. Runtime Assembly

### Owns

- avatar reconstruction
- hoverboard reconstruction
- turtle reconstruction
- material and attachment mapping

### Success Looks Like

- one canonical rig can represent many NFT variants
- missing or unknown traits fail gracefully

## 5. Hub And Social Play

### Owns

- social hub layout
- discoverability of activities
- lightweight social games
- player presence

### Success Looks Like

- the hub is readable, attractive, and not overloaded
- players can naturally move from social play into races or mini-games

## 6. Progression And Meta

### Owns

- XP
- Tickets
- unlocks
- save data
- leaderboards

### Success Looks Like

- progression feels rewarding without depending on token payouts
- state persists cleanly across sessions

## 7. Browser And Performance

### Owns

- WebGL constraints
- asset budgets
- Addressables strategy
- platform-specific fallbacks

### Success Looks Like

- browser builds are intentional and stable
- desktop quality is preserved

## 8. UGC And Creator Tools

### Owns

- track authoring
- modular build pieces
- blueprint persistence
- moderation hooks

### Success Looks Like

- user-generated content starts off-chain and draft-first
- publish and persistence are gated behind moderation or governance rules

## 9. LiveOps And Compliance

### Owns

- safe reward design
- payout boundaries
- moderation and abuse controls
- seasonal event hooks

### Success Looks Like

- the game loop remains legally and operationally sane
- high-risk economy features stay outside the core loop until approved

## Recommended Order Of Work

1. Core Feel
2. Networking And Sessions
3. Progression And Meta
4. Metadata And Ownership
5. Runtime Assembly
6. Hub And Social Play
7. Browser And Performance
8. UGC And Creator Tools
9. LiveOps And Compliance
