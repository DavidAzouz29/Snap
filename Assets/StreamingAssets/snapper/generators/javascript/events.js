goog.require('Blockly.JavaScript');

Blockly.JavaScript['events_start'] = function(block) {
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  // TODO: Assemble JavaScript into code variable.
  var code = 'void Start()\n{\n' + statements_code + '}';
  return code;
};

Blockly.JavaScript['events_update'] = function(block) {
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  // TODO: Assemble JavaScript into code variable.
  var code = 'void Update()\n{\n' + statements_code + '}';
  return code;
};

Blockly.JavaScript['events_fixed_update'] = function(block) {
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  // TODO: Assemble JavaScript into code variable.
  var code = 'void FixedUpdate()\n{\n' + statements_code + '}';
  return code;
};

Blockly.JavaScript['events_awake'] = function(block) {
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  // TODO: Assemble JavaScript into code variable.
  var code = 'void Awake()\n{\n' + statements_code + '}';
  return code;
};

Blockly.JavaScript['events_init'] = function(block) {
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  // TODO: Assemble JavaScript into code variable.
  var code = statements_code + '\n';
  return code;
};

Blockly.JavaScript['events_keypress'] = function(block) {
  var text_keypress = block.getFieldValue('KeyPress');
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  var code = 'if (Input.GetKeyDown(KeyCode.' + pascalCase(text_keypress) + '))\n{\n' + statements_code + '}\n';
  return code;
};

Blockly.JavaScript['events_keyhold'] = function(block) {
  var text_keypress = block.getFieldValue('KeyHold');
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  var code = 'if (Input.GetKey(KeyCode.' + pascalCase(text_keypress) + '))\n{\n' + statements_code + '}\n';
  return code;
};

// If single character e.g. 'w', make 'W'.
// If 'upArrow', make 'UpArrow'
// https://medium.freecodecamp.org/three-ways-to-title-case-a-sentence-in-javascript-676a9175eb27
function pascalCase(str) {
  if(str != "")
  {
    return str.split(' ').map(function(word) {
      return word.replace(word[0], word[0].toUpperCase());
    }).join(' ');
  }
}
