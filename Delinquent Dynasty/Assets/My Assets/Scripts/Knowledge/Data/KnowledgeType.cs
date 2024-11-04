using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo maybe make into dynamic enum if makes sense and not lazy
public enum KnowledgeType {
    NONE = default,
    SECURITY_CODE_ACCESS,
    PHONE_NUMBER,
    LAST_KNOWN_WELLNESS,
    LAST_KNOWN_ATTRIBUTE,
    LAST_KNOWN_TRAIT,
    LAST_KNOWN_SKILL,
    LAST_KNOWN_RELATIONSHIP_STAT,
    LAST_KNOWN_INTEREST,
    EVENT,
    STUDENT_CLASS_PERIOD,
    TEACHER_CLASS_PERIOD,
    ASSIGNED_DORM,
}