Feature: Projection
  A role is defined once, host-free, and projected into each host's native
  agent file with that host's model, effort and tool choices.

  Background:
    Given a checkout with role files under agents/
    And a Claude overlay and a Codex overlay under hosts/
    And a feature template under .specht/templates/

  @B-001
  Scenario: A role projects to a Claude agent file
    Given a role named alpha with a description and a body
    And the Claude overlay maps alpha to a model and an effort
    When the checkout is projected
    Then a Claude agent file for alpha exists
    And its frontmatter carries the name, the description and the overlay keys in overlay order
    And its body is the role body unchanged

  @B-002
  Scenario: A role projects to a Codex agent file
    Given a role named alpha with a description and a body
    And the Codex overlay maps alpha to a model and a reasoning effort
    When the checkout is projected
    Then a Codex agent file for alpha exists
    And its instructions begin with the sentence naming agents/alpha.md as the authoritative source
    And its instructions continue with the role body unchanged
    And it carries the overlay keys

  @B-003
  Scenario: A hostile body survives the Codex encoding
    Given a role whose body contains quotes, backslashes, tabs, accented letters and an emoji
    When the checkout is projected
    Then the Codex agent file parses as TOML
    And the text after the authoritative-source sentence equals the role body

  @B-004
  Scenario: A skill is copied to both hosts
    Given a skill directory holding a SKILL.md, a nested reference file and a binary asset
    When the checkout is projected
    Then every file in that directory exists under the Claude skills directory with the same bytes
    And every file in that directory exists under the Codex discovery directory with the same bytes

  @B-005
  Scenario: Ownership declarations become the companion's table
    Given alpha owns sections 1, 2 and Tasks and beta owns section 3
    And the roles index declares Scoring as derived
    When the checkout is projected
    Then the companion skill carries a section-ownership table between its generated markers
    And the table has one row per template section in template order
    And projecting again leaves the companion unchanged

  @B-006
  Scenario: Two roles claim one section
    Given alpha and beta both own section 2
    When the checkout is projected
    Then projection fails naming section 2, alpha and beta
    And no output directory exists

  @B-007
  Scenario: A template section has no owner
    Given no role and no unowned declaration claims section 3
    When the checkout is projected
    Then projection fails naming section 3
    And no output directory exists

  @B-007
  Scenario: A role claims a section the template lacks
    Given beta owns section 4 and the template has no section 4
    When the checkout is projected
    Then projection fails naming section 4

  @B-008
  Scenario: Check passes when outputs are current
    Given the checkout was projected and nothing changed since
    When the checkout is checked
    Then the check reports no difference

  @B-008
  Scenario: Check names every drift and writes nothing
    Given the checkout was projected
    And one output was edited by hand, one was deleted, and one file in an output directory has no source
    When the checkout is checked
    Then the check names the edited, the deleted and the sourceless path
    And the edited output still holds the hand edit

  @B-009
  Scenario: An overlay names a role with no file
    Given the Codex overlay names a role delta and agents/delta.md does not exist
    When the checkout is projected
    Then projection fails naming delta and the overlay
    And no output directory exists

  @B-009
  Scenario: A role's name differs from its file name
    Given agents/beta.md declares the name gamma
    When the checkout is projected
    Then projection fails naming both names
    And no output directory exists

  @B-024
  Scenario: Source host settings preserve overlay authority
    Given role frontmatter includes optional host settings
    When the checkout is projected
    Then the agent uses its host overlay settings

  @B-017
  Scenario: Personas become the roster
    Given alpha signs as Zed and beta signs as Abe
    When the checkout is projected
    Then agents/PERSONAS.md carries a roster between its generated markers
    And Abe's row comes before Zed's, each linking its role and giving its voice
    And projecting again leaves the roster unchanged

  @B-018
  Scenario: A persona line precedes the body on both hosts
    Given alpha signs as Zed with the voice Terse
    When the checkout is projected
    Then the Claude agent file for alpha opens its body with the line naming Zed, the voice and the role name alpha
    And the Codex instructions carry the same line after the authoritative-source sentence
    And the role body follows that line unchanged on both hosts
    And beta, which has no persona, gets no line

  @B-013
  Scenario: Review units become the registry
    Given the roles index declares the units design and diff, in that order
    And beta reviews the design and alpha reviews the diff
    When the checkout is projected
    Then the companion skill carries a review-pass registry between its generated markers
    And the registry lists design before diff, each with its one reviewer
    And projecting again leaves the companion unchanged

  @B-014
  Scenario: Two roles review one unit
    Given alpha and beta both review the diff
    When the checkout is projected
    Then projection fails naming the diff, alpha and beta
    And no output directory exists

  @B-015
  Scenario: A declared unit no role reviews
    Given no role reviews the design
    When the checkout is projected
    Then projection fails naming the design
    And no output directory exists

  @B-015
  Scenario: A role reviews a unit nobody declared
    Given beta reviews style and the roles index declares no such unit
    When the checkout is projected
    Then projection fails naming beta and style
    And no output directory exists

  @B-016
  Scenario: A section stamp names a role that does not own it
    Given a specification whose Business Goal is stamped by beta
    And alpha owns the Business Goal
    When the checkout is projected
    Then projection fails naming the file, the Business Goal, beta and alpha
    And no output directory exists

  @B-016
  Scenario Outline: A template-style stamp names the wrong writer
    Given a specification whose <section> belongs to <owner>
    And its stamp names <writer>
    When the checkout is projected
    Then projection fails naming the file, the section and both roles
    And no output directory exists

    Examples:
      | section            | owner | writer        |
      | Concern Separation | alpha | implementer   |
      | Technical Design   | alpha | implementer   |
      | Sign-off           | beta  | spec-reviewer |

  @B-016
  Scenario Outline: A fenced example does not declare section ownership
    Given a specification with a correctly owned section stamp
    And a non-owner stamp appears only in a <fence> example
    When the checkout is projected
    Then projection succeeds without treating the example as a specification section

    Examples:
      | fence                                              |
      | tilde-fenced                                       |
      | four-backtick-fenced containing shorter backticks   |

  @B-016
  Scenario: A fenced example does not hide a later ownership violation
    Given a four-backtick example containing shorter backticks
    And a real Business Goal after the example is stamped by beta
    And alpha owns the Business Goal
    When the checkout is projected
    Then projection fails naming the file, the Business Goal, beta and alpha
    And no output directory exists

  @boundary
  Scenario: A role absent from an overlay receives the defaults
    Given a role named beta that no overlay names
    When the checkout is projected
    Then beta's agent file on each host carries that host's default keys

  @B-017
  Scenario: Mixed personas sort named entries before unnamed roles
    Given two named personas Zed and Abe and two unnamed personas
    And a role without a persona
    When the checkout is projected
    Then named rows sort Abe before Zed
    And unnamed rows follow sorted by role name with an em dash for the name
    And every persona row carries its voice and role link
    And the role without a persona has no row
    And projecting again leaves the roster unchanged

  @B-018
  Scenario: An unnamed persona preserves its voice on both hosts
    Given alpha declares the persona voice Terse without a name
    When the checkout is projected
    Then both hosts prepend "Voice: Terse." with no signing instruction
    And the role body follows unchanged on both hosts
    And beta, which has no persona, gets no line on either host

  @B-019
  Scenario: An invalid agent document is rejected
    Given an agent with invalid frontmatter or missing contract sections
    When the checkout is projected
    Then projection fails identifying the document and schema rule

  @B-020
  Scenario: An invalid or missing skill document is rejected
    Given a skill directory with missing or invalid skill instructions
    When the checkout is projected
    Then projection fails identifying the skill and schema rule

  @B-021
  Scenario: Examples cannot supply required sections
    Given required headings appear only in examples, comments or nested blocks
    When the checkout is projected
    Then projection rejects the missing document sections

  @B-022
  Scenario: Rejection preserves existing outputs
    Given the checkout has existing projected outputs
    And a source document or its schema is invalid or missing
    When the checkout is projected or checked
    Then projection fails with a path and field or structure diagnostic
    And every existing output remains unchanged

  @B-023
  Scenario: A tracked schema change takes effect
    Given a tracked schema adds a required document section
    When the checkout is projected
    Then the document fails until it includes that section

  @B-025
  Scenario: Tracker entries do not become skills or skill resources
    Given tracker entries beside skill directories and nested among runtime resources
    When the checkout is projected
    Then tracker entries produce no host skill or resource
    And ordinary hidden and similarly named runtime resources retain their bytes

  @B-008 @B-025
  Scenario: Previously projected tracker records are stale outputs
    Given a host output contains a tracker record absent from the runtime plan
    When projection differences are checked
    Then the tracker record is reported as stale without changing the source tracker
