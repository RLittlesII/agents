Feature: Next work
  A person asking what is next compares Features and their completion paths,
  sees the one actionable step for each and the steps still blocked, and takes
  no work by asking.

  @B-001
  Scenario: Feature comparisons lead the report
    Given two Features with actionable work in separate lanes
    When the person asks which Feature to focus on
    Then the Feature comparisons and completion paths come first
    And the lanes, blockers and items follow them

  @B-002
  Scenario: A Feature comparison keeps its facts apart
    Given a Feature with high importance, much remaining work and two blockers
    When the Features are compared
    Then its stable id, importance, remaining work, blockers, completion path and next available step are named
    And its importance, proximity to completion and blocker count are separate values, not one score

  @B-003
  Scenario: A focused answer includes another Feature's prerequisite
    Given a selected Feature needs work another Feature owns
    When the person asks for the next work on the selected Feature
    Then that work is included
    And its owning Feature and what it advances are named

  @B-004
  Scenario: A focused answer leaves out unrelated work
    Given a selected Feature
    And an unrelated Feature with actionable work that advances none of the selection
    When the person asks for the next work on the selected Feature
    Then the unrelated work is not in the answer

  @B-005
  Scenario: An unknown Feature is a scope error
    Given a requested Feature id that names no Feature
    When the person asks for that Feature's next work
    Then a scope error is reported
    And no other work is offered in its place

  @B-006
  Scenario: A valid scope with no start is explained
    Given a selected Feature whose unfinished work is all blocked or occupied
    When the person asks for that Feature's next work
    Then no start is offered
    And the blockers and occupied work that explain it are named

  @B-007
  Scenario: One set of inputs serves the whole answer
    Given the queue is read once for a request
    And an item changes state after that read
    When the comparison, scores, completion paths and lanes are reported
    Then all four reflect the state as it was read

  @B-008
  Scenario: The report says when its inputs were read
    Given a request for the next work
    When the report is produced
    Then it names when its inputs were read

  @B-009
  Scenario Outline: Each stage can be the next step
    Given work at the <stage> stage whose own inputs permit that stage
    When the person asks what is next
    Then <stage> is stated as its next step

    Examples:
      | stage          |
      | specification  |
      | decomposition  |
      | design         |
      | investigation  |
      | implementation |

  @B-010
  Scenario: Intake does not wait for its own approval
    Given an ask with no agreement yet
    When the person asks what is next
    Then authoring the agreement is offered
    And no approval of that agreement is required to offer it

  @B-011
  Scenario Outline: Design waits for an approved agreement
    Given design work whose agreement sections <approval>
    And an open question <question>
    When the person asks what is next
    Then the design is <result>

    Examples:
      | approval                               | question                           | result   |
      | are approved for its linked claims     | names only claims it does not link | offered  |
      | are not approved for its linked claims | names only claims it does not link | withheld |
      | are approved for its linked claims     | names one of its linked claims     | withheld |

  @B-012
  Scenario: Design runs ahead of an unfinished prerequisite
    Given design work whose agreement is approved for its linked claims
    And no open question blocks those claims
    But a prerequisite of its implementation is unfinished
    When the person asks what is next
    Then the design is offered as the next step

  @B-013
  Scenario Outline: Implementation waits for approval, design and delivery
    Given implementation work that has everything it needs except <missing>
    When the person asks what is next
    Then the implementation is withheld

    Examples:
      | missing                                       |
      | approval of the agreement and design sections |
      | a design marked Ready                         |
      | freedom from a question blocking its claims   |
      | a delivered prerequisite                      |

  @B-014
  Scenario: A status is not approval
    Given a specification whose maturity reads approved
    And an item whose status reads ready
    But the sign-off rows for the agreement sections are not approved
    When the person asks what is next
    Then the design gate is treated as unmet

  @B-015
  Scenario: An unrelated question blocks nothing
    Given design work whose agreement is approved for its linked claims
    And an open question whose row names only a claim the work does not link
    When the person asks what is next
    Then the design is offered

  @B-016
  Scenario Outline: A spike needs a bounded question and a time box
    Given a spike that <states>
    When the person asks what is next
    Then the investigation is <result>

    Examples:
      | states                             | result   |
      | a bounded question and a time box  | offered  |
      | a bounded question but no time box | withheld |
      | a time box but no bounded question | withheld |

  @B-017
  Scenario Outline: Taken work occupies its lane
    Given taken work <where> the requested scope
    And the taken work is <stage>
    And selected work that needs the same lane
    When the person asks what is next
    Then the taken work is shown as occupied
    And no new start is offered in that lane

    Examples:
      | where   | stage       |
      | inside  | in progress |
      | outside | in progress |
      | inside  | in review   |

  @B-018
  Scenario Outline: Only work finished as completed is delivered
    Given a prerequisite that is <state>
    When its dependant's readiness is evaluated
    Then the prerequisite is <result>

    Examples:
      | state                 | result      |
      | finished as completed | delivered   |
      | in review             | undelivered |
      | closed as not planned | undelivered |
      | reopened              | undelivered |

  @B-019
  Scenario: Waiting is derived, not recorded
    Given an item whose status reads ready
    But one of the items it waits on is unfinished
    When the person asks what is next
    Then the item is treated as waiting

  @B-020
  Scenario: A container is not offered while it has open children
    Given a container with two unfinished children
    When candidates are chosen
    Then the children are candidates
    And the container is not offered

  @B-021
  Scenario: Authored inputs win over a stored derived value
    Given a stored rank and list of dependants that disagree with what the items say they wait on
    When the next work is derived
    Then the order follows what the items say they wait on

  @B-022
  Scenario: Priority tier comes before completion score
    Given a Feature in a higher priority tier with a lower completion score
    And a Feature in a lower tier with a higher completion score
    When ranked work is ordered
    Then the higher-tier Feature's work comes first

  @B-023
  Scenario: A shared prerequisite is counted once
    Given a Feature with value 4
    And two unfinished children with cost 3 and 2
    And both children wait on the same unfinished prerequisite with cost 8
    When the Feature's completion score is calculated
    Then the score is 4 divided by 13

  @B-024
  Scenario Outline: Work in progress keeps its full cost
    Given an item with cost 8 that is <state>
    When its Feature's unfinished closure is costed
    Then the item contributes <cost>

    Examples:
      | state                 | cost |
      | in progress           | 8    |
      | in review             | 8    |
      | finished as completed | 0    |

  @B-025
  Scenario: A prerequisite inherits the strongest selected Feature
    Given a prerequisite that advances two selected Features in different tiers
    When scoped work is ranked
    Then the prerequisite takes the stronger tier
    And the best score within that tier

  @B-026
  Scenario: An unselected Feature does not move a focused answer
    Given a prerequisite that advances a selected Feature
    And also advances an unselected Feature in a higher tier
    When scoped work is ranked
    Then the prerequisite ranks as it would if the unselected Feature did not exist

  @B-027
  Scenario Outline: Ties break on waiting work, then stable id
    Given two items with equal tier and equal exact score
    And <waiting>
    When they are ordered
    Then <first> comes first

    Examples:
      | waiting                                           | first                             |
      | three open items wait on one and one on the other | the item with three waiting       |
      | one open item waits on each                       | the item with the lower stable id |

  @B-028
  Scenario Outline: Missing inputs leave a score uncomputed
    Given a Feature whose closure has <gap>
    When completion scores are calculated
    Then the Feature's score is uncomputed
    And it is neither zero nor infinite

    Examples:
      | gap                                                             |
      | no value assessment                                             |
      | an item with no cost                                            |
      | an unknown decomposition                                        |
      | a dependency cycle                                              |
      | an active claim with neither planned work nor verified delivery |

  @B-029
  Scenario: Independent work is still ranked
    Given a Feature whose closure is missing a cost
    And an independent Feature whose evidence is complete
    When the person asks what is next
    Then the independent Feature is ranked and its work offered

  @B-030
  Scenario Outline: Unverified delivery state offers no start
    Given work whose delivery state is <state>
    When the person asks what is next
    Then no start is offered for it

    Examples:
      | state             |
      | missing           |
      | ambiguous         |
      | unable to be read |

  @B-031
  Scenario: A claim link is not inferred from prose
    Given an item whose prose mentions a claim
    But whose claim assignment names a claim that does not exist
    When its readiness is evaluated
    Then the item is treated as linking no claim

  @B-032
  Scenario Outline: Gaps in scope are named
    Given <gap> within the requested scope
    When the report is produced
    Then the report names it and the work it affects

    Examples:
      | gap                |
      | a dependency cycle |
      | a missing link     |
      | a decision hold    |
      | occupied work      |

  @B-033
  Scenario: Delivered children alone do not finish a Feature
    Given a Feature whose known children are all delivered
    And one active claim with neither planned work nor verified delivery
    When the person asks for that Feature's next work
    Then the Feature is not reported as finished

  @B-034
  Scenario: One head per available lane
    Given two independent available lanes in different Features with several startable items each
    When the person asks what is next
    Then one ranked startable head is offered for each lane

  @B-035
  Scenario: A shared edit joins two lanes
    Given two items with independent dependencies
    But evidence that both edit the same generated output
    When lanes are inferred
    Then both items are in one lane

  @B-036
  Scenario: An inferred conflict carries its evidence and uncertainty
    Given two items placed in one lane by an inferred shared edit
    When the report is produced
    Then the evidence for the conflict is stated
    And so is how certain it is

  @B-037
  Scenario: Unscored work is not reported as blocked
    Given actionable work with no cost
    When the report is produced
    Then the work is listed apart from the ranked lane heads
    And it is not listed as blocked

  @B-038
  Scenario: Standalone work stays visible
    Given an actionable chore that belongs to no Feature
    When the person asks what is next
    Then the chore appears in the report where the companion directs

  @B-039
  Scenario: Expanding the report does not change its choices
    Given a report produced from one set of inputs
    When the person asks for more detail on it
    Then the scope, readiness, scores and lane heads are unchanged

  @B-040 @boundary
  Scenario: Asking what is next takes no work
    Given a person asks what is next
    When the report is produced
    Then no item is claimed and no record is edited
    And no derived score, tier or gate is written back
    And no branch, worktree or dispatch is started

  @B-041
  Scenario: A dependency cycle goes to grooming as a re-cut
    Given two items that depend on each other
    When the report is produced
    Then the cycle is routed to grooming as a re-cut
    And no dependency edge is dropped to break it

  @B-042
  Scenario Outline: The tracker wins over a mirrored status
    Given work whose specification mirrors a status of <mirror>
    And the tracker <tracker>
    When the person asks what is next
    Then <outcome>

    Examples:
      | mirror  | tracker                    | outcome                                                          |
      | ready   | shows the work as taken    | the work is shown as occupied and the disagreement is named      |

  @B-043
  Scenario: A duplicate id is named, not resolved
    Given two records that claim the same stable id
    And independent work that depends on neither
    When the report is produced
    Then the duplicate is named as a diagnostic
    And neither record is chosen
    And no start is offered for work that depends on either record
    And the independent work is still offered

  @B-044
  Scenario: A question naming no claim leaves readiness unverified
    Given design work whose agreement is approved for its linked claims
    And an open question in its Feature whose row names no blocked claim
    When the person asks what is next
    Then the design is not offered
    And the report names the question as a gap in readiness

  @B-045
  Scenario: A finished Feature is not ranked
    Given a Feature with no unfinished work in its closure
    And every active claim of that Feature delivered
    When the Features are compared
    Then the Feature is reported as finished
    And it is left out of the ranking

  @B-046
  Scenario: One start per Feature
    Given two startable items in one Feature with no shared edit
    When the person asks what is next
    Then the items are placed in separate lanes
    And only the higher-ranked item is offered as a start
    And the other is shown queued behind it

  @B-046
  Scenario: A Feature with taken work offers no further start
    Given a Feature with taken work
    And another startable item in that Feature with no shared edit
    When the person asks what is next
    Then no start is offered for that Feature
    And the startable item is shown queued
