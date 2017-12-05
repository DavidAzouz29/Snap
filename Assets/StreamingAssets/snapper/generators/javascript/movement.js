goog.require('Blockly.JavaScript');

Blockly.JavaScript['transform_transform'] = function(block) {
  var value_move = Blockly.JavaScript.valueToCode(block, 'Move', Blockly.JavaScript.ORDER_MEMBER);
  // TODO: Assemble JavaScript into code variable.
  var code = 'transform.' + value_move + ';\n';
  return code;
};

Blockly.JavaScript['math_vector3'] = function(block) {
  var number_x = block.getFieldValue('X');
  var number_y = block.getFieldValue('Y');
  var number_z = block.getFieldValue('Z');
  var value_v3 = Blockly.JavaScript.valueToCode(block, 'v3', Blockly.JavaScript.ORDER_NEW);
  // TODO: Assemble JavaScript into code variable.
  //var code = 'new Vector3({0}, {1}, {2})'.format(number_x, number_y, number_z);
  var code = 'Vector3(' + number_x + ', ' + number_y + ', ' + number_z + ')'; //;\n
  // TODO: Change ORDER_NONE to the correct strength.
  return [code, Blockly.JavaScript.ORDER_ASSIGNMENT];
};

Blockly.JavaScript['math_arithmetic_operator'] = function(block) {
  // Basic arithmetic operators, and power.
  var OPERATORS = {
    'plusEqual': [' += ', Blockly.JavaScript.ORDER_ASSIGNMENT],
    'minusEqual': [' -= ', Blockly.JavaScript.ORDER_ASSIGNMENT],
    'timesEqual': [' *= ', Blockly.JavaScript.ORDER_ASSIGNMENT],
    'divideEqual': [' /= ', Blockly.JavaScript.ORDER_ASSIGNMENT],
    'modulusEqual': [' %= ', Blockly.JavaScript.ORDER_ASSIGNMENT]
  };
  var tuple = OPERATORS[block.getFieldValue('OP')];
  var operator = tuple[0];
  var order = tuple[1];
  var argument0 = Blockly.JavaScript.valueToCode(block, 'A', order) || '';
  var argument1 = Blockly.JavaScript.valueToCode(block, 'B', order) || '0';
  var code = argument0 + operator + argument1;
  return [code, Blockly.JavaScript.ORDER_ASSIGNMENT];
};

Blockly.JavaScript['move_move'] = function(block) {
  var value_vector = Blockly.JavaScript.valueToCode(block, 'Vector', Blockly.JavaScript.ORDER_NONE);
  var dropdown_speed = block.getFieldValue('speed');
  // TODO: Assemble JavaScript into code variable.
  if(value_vector == null)
    value_vector = 'Vector3.forward'
    var speed = '0f';
    switch (dropdown_speed) {
      case 'speedSlow':
        speed = '2f';
        break;
        case 'speedMedium':
          speed = '5f';
          break;
          case 'speedFast':
            speed = '10f';
            break;
      default:
      speed = '2f';
      break;
    }
  var code = 'transform.Translate(' + value_vector + ' * Time.deltaTime * ' + speed + ');\n';
  return code;
};

Blockly.JavaScript['move_rotate'] = function(block) {
  var dropdown_orientation = block.getFieldValue('orientation');
  var value_vector = Blockly.JavaScript.valueToCode(block, 'Vector', Blockly.JavaScript.ORDER_NONE);
  var dropdown_speed = block.getFieldValue('speed');
  // TODO: Assemble JavaScript into code variable.
  var ori = '';
  switch (dropdown_orientation) {
    case 'clockwise':
      ori = '';
      break;
      case 'anticlockwise':
        ori = '-';
        break;
    default:
      ori = '';
      break;
  }

  if(value_vector == null)
    value_vector = 'Vector3.up'

    var speed = '0';
    switch (dropdown_speed) {
      case 'speedSlow':
        speed = '90';
        break;
        case 'speedMedium':
          speed = '180';
          break;
          case 'speedFast':
            speed = '270';
            break;
      default:
      speed = '2';
      break;
    }
  var code = 'transform.Rotate(' + ori + value_vector + ' * Time.deltaTime * ' + speed + 'f);\n';
  return code;
};

Blockly.JavaScript['move_rotate_angle'] = function(block) {
  var dropdown_orientation = block.getFieldValue('orientation');
  var value_vector = Blockly.JavaScript.valueToCode(block, 'Vector', Blockly.JavaScript.ORDER_NONE);
  var angle_angle = block.getFieldValue('angle');
  // TODO: Assemble JavaScript into code variable.
  // TODO: Assemble JavaScript into code variable.
  var ori = '';
  switch (dropdown_orientation) {
    case 'clockwise':
      ori = '';
      break;
      case 'anticlockwise':
        ori = '-';
        break;
    default:
      ori = '';
      break;
  }

  if(value_vector == null)
    value_vector = 'Vector3.up'

  var code = 'transform.Rotate(' + ori + value_vector + ' * Time.deltaTime * ' + angle_angle + 'f);\n';
  return code;
};

Blockly.JavaScript['move_scale'] = function(block) {
  var dropdown_scale = block.getFieldValue('scale');
  var scale = '0f';
  switch (dropdown_scale) {
    case 'scaleSmall':
      scale = '0.5f, 0.5f, 0.5f';
      break;
      case 'scaleMedium':
        scale = '1f, 1f, 1f';
        break;
        case 'scaleBig':
          scale = '2f, 2f, 2f';
          break;
    default:
    scale = '1f, 1f, 1f';
    break;
  }
  // TODO: Assemble JavaScript into code variable.
  var code = 'transform.localScale += new Vector3(' + scale + ');\n';
  return code;
};

Blockly.JavaScript['move_facing'] = function(block) {
  var dropdown_direction = block.getFieldValue('direction');
  var value_directions = Blockly.JavaScript.valueToCode(block, 'directions', Blockly.JavaScript.ORDER_NONE);
  // TODO: Assemble JavaScript into code variable.
  var code = 'forward';
  switch (dropdown_direction) {
    case 'Forward':
      code = 'forward';
      break;
      case 'Up':
        code = 'up';
        break;
        case 'Right':
          code = 'right';
          break;
      break;
    default:
      code = 'forward';
      break;
  }
  code = 'Vector3.' + code;
  if(value_directions != "")
  {
    code += ' * ' + value_directions;
  }
  // TODO: Change ORDER_NONE to the correct strength.
  return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['move_speed'] = function(block) {
  var dropdown_speed = block.getFieldValue('speed');
  // TODO: Assemble JavaScript into code variable.
  var code = '0f';
  switch (dropdown_speed) {
    case 'slow':
      code = '2f';
      break;
      case 'medium':
        code = '5f';
        break;
        case 'fast':
          code = '10f';
          break;
    default:
    code = '2f';
  }
  // TODO: Change ORDER_NONE to the correct strength.
  return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['move_direction'] = function(block) {
  var dropdown_orientation = block.getFieldValue('orientation');
  var angle_angle = block.getFieldValue('angle');
  var value_vector = Blockly.JavaScript.valueToCode(block, 'Vector', Blockly.JavaScript.ORDER_NONE);
  // TODO: Assemble JavaScript into code variable.
  var ori = '';
  switch (dropdown_orientation) {
    case 'clockwise':
      ori = '';
      break;
      case 'anticlockwise':
        ori = '-';
        break;
    default:
      ori = '';
      break;
  }
  var code = 'transform.localRotation = Quaternion.Slerp(transform.localRotation, ' +
            'Quaternion.AngleAxis(' + ori + angle_angle + 'f, ' + value_vector + '), ' +
            'Time.deltaTime * ' + angle_angle + 'f / 36f);\n';
  return code;
};
