Feature: Role contract
  A role file is a host-free contract: fixed frontmatter keys, four body
  headings, and no product technology.

  Background:
    Given the role files under agents/
    And the roles index agents/README.md
    And the specification template under .specht/templates/

  @B-001
  Scenario: Every role declares the contract keys
    When the role files are loaded
    Then each declares a name equal to its file name
    And each declares a one-line description
    And each declares a semver version
    And each declares its owned specification sections
    And none carries a model, an effort, a tool list or a reasoning effort

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

  @B-007
  Scenario: A persona carries a name and a voice
    Given a role declaring a persona with a name and no voice
    When the checkout is projected
    Then projection fails naming the role and the missing voice
    And no output directory exists

  @B-007
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

  @boundary
  Scenario: A role owns nothing
    Given a role whose owned specification sections are empty
    When the role files are loaded
    Then the role loads
    And it has no row in the ownership table
