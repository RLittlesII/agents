Feature: Role contract
  A role file is a host-free contract: fixed frontmatter keys, four body
  headings, and no product technology.

  Background:
    Given the role files under agents/
    And the roles index agents/README.md
    And the specification template under .specht/templates/

  @B-041
  Scenario: Every role declares the contract keys
    When the role files are loaded
    Then each declares a name equal to its file name
    And each declares a nonblank description
    And each declares a semver version
    And each declares its owned specification sections
    And optional metadata and host fields pass schema validation
    And host overlays remain authoritative for rendered settings

  @B-002
  Scenario: Every template section has exactly one owner
    Given the sections each role owns
    And the sections the roles index declares as owned by nobody in particular
    When the ownership table is computed against the template
    Then every template section has exactly one owner
    And no owner names a section the template lacks

  @B-003
  Scenario: Every role body carries the four headings
    When the role files are loaded
    Then each body carries the headings Owns, Read first and Refuse
    And each body carries Produce or Look for

  @B-005
  Scenario: A reviewing role states its report
    Given a role whose body carries the heading Look for
    When the role files are loaded
    Then its body also carries the heading Report

  @B-006
  Scenario: A role links only skills that exist
    Given a role whose body links a skill with no directory under skills/
    When the checkout is projected
    Then projection fails naming the role and the skill
    And no output directory exists

  @B-040
  Scenario: A persona requires a voice
    Given a role declaring a persona with a name and no voice
    When the checkout is projected
    Then projection fails naming the role and the missing voice
    And no output directory exists

  @B-040
  Scenario: Two roles sign with one name
    Given alpha and beta both declare the persona Abe
    When the checkout is projected
    Then projection fails naming Abe, alpha and beta
    And no output directory exists

  @B-008
  Scenario: A reviewing role carries the reviewing headings
    Given a role declaring that it reviews the diff
    And its body has no Look for or Report heading
    When the checkout is projected
    Then projection fails naming the role
    And no output directory exists

  @B-009
  Scenario: A review unit declares its trigger and refusal
    Given a review unit in the roles index that says what it reviews and when its pass runs
    And it does not say what the pass never does
    When the checkout is projected
    Then projection fails naming the unit and the missing key
    And no output directory exists

  @B-004
  Scenario: A role body names no product technology
    Given the names of a consuming project's language, libraries, paths and build tools
    When a reviewer searches the role bodies for them
    Then none appears
    And the companion skill of the consuming repository is where those names live

  @B-002 @boundary
  Scenario: A role owns nothing
    Given a role whose owned specification sections are empty
    When the role files are loaded
    Then the role loads
    And it has no row in the ownership table

  @B-010
  Scenario Outline: Each state has one accountable lead
    Given work exercising the <state> responsibility
    When the collaboration is assigned
    Then <lead> leads that state

    Examples:
      | state       | lead          |
      | Specify     | spec-author   |
      | Design      | architect     |
      | Implement   | implementer   |
      | Verify      | spec-reviewer |
      | Release     | release       |
      | Investigate | bug-finder    |

  @B-011
  Scenario: Specialists join for bounded relevant work
    Given affected claims require business rules and presentation changes
    And the interaction is already decided
    When the implementation lead assigns specialist work
    Then domain receives the business-rule contribution
    And ui receives the presentation contribution
    And each contribution names its affected claims
    And unrelated specialists are not required

  @B-011
  Scenario: Rendering an unchanged model needs no modeling assignment
    Given presentation work renders an existing domain model
    And no business invariant, rule or domain concept needs modeling or change
    When the lead selects the required expertise
    Then ui receives the presentation contribution
    And domain is not required merely because the model is rendered

  @B-012
  Scenario: The lead integrates specialist contributions
    Given several specialists contribute to one implementation
    When their bounded assignments finish
    Then implementer remains accountable for the integrated change
    And completed assignments alone do not establish that the change satisfies its claims

  @B-013
  Scenario: Lead accountability preserves section ownership
    Given database finds a missing design decision
    When implementer coordinates the affected change
    Then the design question returns to architect
    And neither implementer nor database writes the missing design section

  @B-014
  Scenario: A bounded change reuses valid earlier artifacts
    Given a correction has an agreed claim and approved applicable design
    And the accepted diagnosis and regression test are ready
    When the next responsibility is selected
    Then the work enters implementation
    And discovery and design authoring are not repeated

  @B-014
  Scenario: Reusing an agreement does not waive an unmet gate
    Given an existing claim describes a requested correction
    But the applicable design approval is missing
    When the next responsibility is selected
    Then implementation waits for the missing approval

  @B-015
  Scenario: Discovery belongs to the specification author
    Given a raw ask has two materially different readings
    When spec-author works in discovery mode
    Then the separating question is recorded before either reading becomes a claim
    And the confirmed need becomes the agreement input
    And no standalone analyst is required

  @B-016
  Scenario: The implementer owns a diagnosed correction
    Given a proven cause has an accepted diagnosis
    And a regression test fails for that cause
    When implementer works in correction mode
    Then the correction is bounded by that cause and the existing design
    And regression checks cover the affected behavior
    And a revealed gap receives a lesson
    And any identified systemic site is corrected or recorded for separate work

  @B-016
  Scenario: Correction waits for proof of the failure
    Given a proposed correction has no proven cause
    And no regression test fails for that cause
    When implementation is considered
    Then correction mode refuses to start
    And the missing investigation and test are requested from their owners

  @B-017
  Scenario: Obstruction removal returns to the blocked work
    Given a build obstruction blocks an implementation
    When the implementation lead assigns build to remove that obstruction
    Then the original implementation remains owned by its lead
    And proof that it can resume completes the bounded assignment
    And adjacent improvements are recorded separately

  @B-018
  Scenario: Pairing is a mode of the active role
    Given the person requests visible steps while a specification is authored
    When spec-author uses the collaboration mode
    Then the next step and its result are visible
    And a decision fork pauses for the person's choice
    And the decision is recorded
    And the mode does not authorize production code

  @B-019
  Scenario: Investigation includes correlated log analysis
    Given an incident has logs for several concurrent activities
    When bug-finder applies log analysis
    Then the relevant window and activity are isolated
    And the event sequence cites its decisive evidence
    And inferences and evidence gaps are identified
    And secrets are not repeated

  @B-019
  Scenario: Large log evidence is a bounded task
    Given incident evidence is too large for one analysis pass
    When bug-finder assigns part of its analysis
    Then that assignment returns a bounded evidence account
    And bug-finder retains responsibility for the diagnosis
    And no standalone log-parser role is required

  @B-020
  Scenario: A different reviewer performs design review
    Given architect has authored a design
    When spec-reviewer performs design-review mode
    Then the reviewing instance did not author that design
    And it returns findings against the claims and constraints
    And it does not rewrite the design

  @B-020
  Scenario: Consolidating review roles does not allow self-review
    Given an instance authored the design under consideration
    When that instance offers to perform its design review
    Then the review is assigned to a different instance

  @B-021
  Scenario Outline: Database expertise joins an affected state
    Given persistence is affected during <state>
    When the lead selects the required expertise
    Then database receives the <contribution> assignment
    And database does not become a mandatory stage of unrelated work

    Examples:
      | state       | contribution                          |
      | Design      | persistence design advice             |
      | Implement   | persistence implementation            |
      | Investigate | stored-data failure investigation     |
      | Release     | data compatibility and recovery checks |

  @B-022
  Scenario: Database decisions follow agreed invariants
    Given a storage choice would permit a state forbidden by an agreed business invariant
    When database evaluates that choice
    Then the choice is rejected against the invariant
    And an unresolved design question returns to architect
    And database does not invent a replacement business rule

  @B-023
  Scenario: Changing existing data requires recovery evidence
    Given a migration or backfill changes existing stored information
    When database completes its contribution
    Then the contribution demonstrates compatibility with the affected serving versions
    And it verifies a rollback or recovery path
    And a destructive change identifies the backup and potential loss

  @B-024
  Scenario Outline: Measurements follow their purpose
    Given the recorded need is <need>
    When the lead selects measurement expertise
    Then the assignment goes to <specialist>

    Examples:
      | need                                               | specialist  |
      | measure a stated performance or cost concern        | benchmarker |
      | compare operational signals for a rollout decision  | observability |

  @B-024
  Scenario: Existing telemetry does not require a benchmark
    Given operational telemetry exists
    And no written performance, cost or structural concern needs measurement
    When the lead considers benchmarking
    Then telemetry alone does not justify a benchmarker assignment

  @B-025
  Scenario: Production instrumentation follows implementation ownership
    Given observability identifies a signal required for an operational decision
    When production instrumentation must change
    Then implementer or its assigned implementation specialist changes the code
    And observability supplies the signal requirement and comparison
    And benchmarker does not change production code to make its measurement possible

  @B-026
  Scenario: A bug follows the existing agreement
    Given reported behavior contradicts an existing claim
    And the applicable design covers the correction
    When the bug is handled
    Then bug-finder proves the cause against that claim
    And a regression test pins the proven cause
    And the accepted diagnosis precedes the correction handoff
    And implementer corrects the failure after the regression test fails
    And spec-reviewer independently verifies the change
    And the agreement is not rewritten merely because a bug was reported

  @B-026
  Scenario Outline: A bug exposes a missing prerequisite
    Given a bug investigation establishes <gap>
    When the correction route is selected
    Then the gap returns to <owner>
    And implementation waits for the applicable prerequisite

    Examples:
      | gap                             | owner       |
      | missing or disputed behavior    | spec-author |
      | a required design change        | architect   |

  @B-027
  Scenario: A new feature follows artifact dependencies
    Given a new feature's intended behavior is accepted
    When the feature is developed
    Then the agreement and scenarios precede its design
    And the applicable approved design precedes failing tests
    And failing tests precede production implementation
    And independent verification precedes release
    And only relevant specialists join the lead's work

  @B-027
  Scenario: A feature without persistence needs no database assignment
    Given a feature changes only an already decided presentation
    And no persistence concern is affected
    When the lead selects expertise
    Then ui receives the presentation work
    And database is not required

  @B-028
  Scenario Outline: Technical obstacles select relevant investigation
    Given current work is blocked by <obstacle>
    When the lead selects assistance
    Then <role> handles the bounded <question>
    And the original lead retains the parent work
    And the result returns to that work

    Examples:
      | obstacle                  | role       | question             |
      | an unknown approach       | architect  | feasibility          |
      | unexpected behavior       | bug-finder | failure cause        |
      | a broken tool or gate     | build      | build obstruction    |

  @B-029
  Scenario: An exploratory idea stays in discovery
    Given a person pitches an idea whose need has not been accepted
    When spec-author explores its intended outcome
    Then unanswered requirements remain questions
    And the idea does not authorize production implementation
    And exploratory wording is not treated as an agreed claim

  @B-029
  Scenario: An idea can require bounded feasibility evidence
    Given accepting an idea depends on a technical feasibility question
    When architect investigates that bounded question
    Then the result returns to discovery as decision evidence
    And the investigation does not authorize speculative production implementation

  @B-030
  Scenario: A production incident uses available evidence and recovery
    Given a production failure has logs and operational telemetry
    And an existing recovery procedure applies
    When the incident is handled
    Then bug-finder coordinates diagnosis using the evidence
    And observability compares the relevant operational signals
    And release coordinates recovery under the existing procedure and confirmation rules
    And diagnosis and recovery retain their separate responsibilities

  @B-030
  Scenario: Operational recovery does not invent a software correction
    Given a proven configuration problem is resolved by the existing recovery procedure
    And no software change is required
    When service is verified as recovered
    Then the production route completes without inventing a production-code assignment

  @B-030
  Scenario: A production failure can require the correction route
    Given production diagnosis proves software violates an existing claim
    When the required correction is selected
    Then it follows the accepted diagnosis and failing regression test route
    And recovery does not replace independent verification of that correction

  @B-031
  Scenario: Missing telemetry remains an evidence gap
    Given an incident has no relevant logs or operational telemetry
    When bug-finder assesses the available evidence
    Then the missing evidence is recorded as a gap
    And unsupported causes remain unproven
    And lack of a signal is not treated as absence of the failure

  @B-032
  Scenario: Changed shipped behavior amends the existing agreement
    Given a requested change replaces behavior of a shipped claim
    When spec-author amends the agreement
    Then the old claim is marked withdrawn without renumbering
    And the replacement behavior receives a new claim identifier and scenarios
    And design impact, dependencies and compatibility are assessed before implementation
    And revised failing tests precede the change
    And independent verification precedes release

  @B-032
  Scenario: A shipped amendment uses only affected specialists
    Given a shipped requirement changes interaction but preserves existing stored information
    When its amendment is assessed
    Then ux helps decide the changed interaction
    And a database migration is not invented

  @B-033
  Scenario: The simplified roster exposes nineteen roles
    When the resulting role roster is inspected
    Then it contains exactly the six state leads and thirteen specialists in the agreement
    And database is a standalone specialist
    And the six retired roles are capabilities or modes of retained roles

  @B-034
  Scenario: Active references follow the consolidated roles
    Given commands and host configurations previously referenced retired roles
    When the consolidated roster is delivered
    Then active contracts and convening references resolve to retained roles and applicable modes
    And projected outputs are regenerated from those sources
    And projected outputs contain no stale standalone roles

  @B-035
  Scenario: Historical evidence survives retirement
    Given a historical decision or review names a retired role
    When active role references are migrated
    Then the historical identity and evidence remain recorded
    And existing claim identifiers keep their anchors

  @B-036
  Scenario: Handoff guidance answers who is involved and when
    Given a reader starts from one of the six agreed work situations
    When the reader consults the roles index
    Then bounded guidance identifies its lead, relevant expertise and artifact dependencies
    And the reader need not traverse a global peer-to-peer role graph


  @B-037
  Scenario: An optional observability subsection has its specialist owner
    Given a consuming specification includes Observability under Technical Design
    When its observability requirements are authored
    Then observability owns that subsection
    And its owner note and author stamp name observability

  @B-037 @boundary
  Scenario: A specification can omit the observability subsection
    Given a consuming specification has no Observability subsection
    When specification ownership is checked
    Then that absence is valid
    And the subsection is not created merely to satisfy ownership

  @B-037
  Scenario: A parent author cannot stamp the observability subsection
    Given a consuming specification includes Observability under Technical Design
    And that subsection is stamped as written by architect
    When ownership stamps are checked
    Then the incorrect owner stamp is rejected
    And the discrepancy names the observability owner

  @B-037 @boundary
  Scenario: Optional ownership does not take over unrelated design
    Given a consuming specification has another subsection under Technical Design
    When that subsection's owner is resolved
    Then architect remains its parent owner
    And observability ownership is confined to Observability
    And benchmarker retains Performance under Testing Strategy

  @B-038
  Scenario: Portable signal declarations identify the boundary and claim
    Given an operational boundary needs an observable outcome
    When observability authors its signal declaration
    Then the declaration uses Boundary, Signal, Level, Template, Fields and Claim in that order
    And each signal is a log, span or metric
    And only a log carries a severity level
    And each template is a constant message, span name or instrument name
    And fields name the recorded information without providing runtime values
    And each declaration cites the claims it serves

  @B-038
  Scenario: A reason supports the absence of relevant signals
    Given no operational signal is required for the affected boundary
    When observability records its contribution
    Then the subsection states None with its reason
    And an unexplained None is incomplete

  @B-038
  Scenario: Runtime values do not replace portable signal requirements
    Given a proposed signal declaration contains captured runtime values
    And its signal name changes with each observed event
    When observability reviews the declaration
    Then that declaration is returned for constant naming and field names only

  @B-038
  Scenario: Signal declarations preserve implementation and design boundaries
    Given an observability contribution requires instrumentation
    When observability completes the declaration
    Then every emitted boundary signal is declared and asserted in a test
    And observability hands its declarations to test-writer for those assertions
    And observability writes neither test code nor production code
    And production code remains assigned to implementer or its implementation specialist
    And parent design questions return to architect
    And performance measurements return to benchmarker
    And observability writes no review verdict

  @B-039
  Scenario: The rename preserves the nineteen-role roster
    Given the nineteen-role roster contains metric-nerd
    When its operational specialist is renamed to observability
    Then the roster still contains nineteen roles
    And active contracts, commands and host references resolve to observability
    And the regenerated outputs expose observability instead of metric-nerd

  @B-039
  Scenario: Stale outputs from the former identity fail the projection check
    Given the active role identity is observability
    And a projected standalone metric-nerd output remains
    When projection drift is checked
    Then the stale output is rejected

  @B-039
  Scenario: Renaming an active role preserves historical evidence
    Given a recorded decision or review names metric-nerd
    When its active role identity becomes observability
    Then the historical identity and evidence remain recorded
    And the existing claim identifiers keep their anchors

  @B-040
  Scenario: A persona name is optional
    Given alpha and beta each declare a persona voice without a name
    When the checkout is projected
    Then both roles load with their voices
    And the unnamed personas do not conflict

  @B-040
  Scenario Outline: A supplied persona name must be a nonblank single line
    Given a role declaring a persona voice and a name that is <invalid>
    When the checkout is projected
    Then projection fails naming the role and persona.name
    And no output directory exists

    Examples:
      | invalid         |
      | empty           |
      | whitespace only |
      | multiline       |
      | carriage return |
      | a list          |

  @B-040
  Scenario Outline: A persona voice must be a nonblank single line
    Given a role declaring a persona without a name and a voice that is <invalid>
    When the checkout is projected
    Then projection fails naming the role and persona.voice
    And no output directory exists

    Examples:
      | invalid         |
      | empty           |
      | whitespace only |
      | multiline       |
      | carriage return |
      | a list          |
