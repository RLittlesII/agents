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

  @B-010
  Scenario Outline: A role file carries a host key
    Given a role file carrying the key <key>
    When the checkout is projected
    Then projection fails naming <key> and the hosts directory
    And no output directory exists

    Examples:
      | key                    |
      | model                  |
      | effort                 |
      | tools                  |
      | model_reasoning_effort |

  @B-011
  Scenario: Personas become the roster
    Given alpha signs as Zed and beta signs as Abe
    When the checkout is projected
    Then agents/PERSONAS.md carries a roster between its generated markers
    And Abe's row comes before Zed's, each linking its role and giving its voice
    And projecting again leaves the roster unchanged

  @B-012
  Scenario: A persona line precedes the body on both hosts
    Given alpha signs as Zed with the voice Terse
    When the checkout is projected
    Then the Claude agent file for alpha opens its body with the line naming Zed, the voice and the role name alpha
    And the Codex instructions carry the same line after the authoritative-source sentence
    And the role body follows that line unchanged on both hosts
    And beta, which has no persona, gets no line

  @boundary
  Scenario: A role absent from an overlay receives the defaults
    Given a role named beta that no overlay names
    When the checkout is projected
    Then beta's agent file on each host carries that host's default keys
