import { useState } from 'react'
import { api, type SpellEditorDocument, type StatsDocument } from '../../api/client'
import { FormField, FormRow, FormSection } from '../../components/FormField'
import { useEditorStore } from '../../store'

interface Props {
  filePath: string
}

export function SpellEditor({ filePath }: Props) {
  const { openTabs, updateTabContent, markTabSaved } = useEditorStore()
  const tab = openTabs.find((t) => t.filePath === filePath)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  if (!tab) return null

  const doc: SpellEditorDocument = JSON.parse(tab.content)

  function update(patch: Partial<SpellEditorDocument>) {
    updateTabContent(filePath, JSON.stringify({ ...doc, ...patch }, null, 2))
  }

  function patchStats(key: 'statModifiers' | 'statRequirements', field: keyof StatsDocument, val: string) {
    const n = parseInt(val, 10)
    update({ [key]: { ...doc[key], [field]: isNaN(n) ? 0 : n } })
  }

  async function handleSave() {
    setSaving(true)
    setError(null)
    try {
      await api.spells.save(doc)
      markTabSaved(filePath)
    } catch (e) {
      setError(String(e))
    } finally {
      setSaving(false)
    }
  }

  const statFields: (keyof StatsDocument)[] = ['strength', 'intelligence', 'dexterity', 'defense', 'vitality']

  function StatGroup({ statsKey }: { statsKey: 'statModifiers' | 'statRequirements' }) {
    return (
      <FormRow>
        {statFields.map((f) => (
          <FormField key={f} label={f.charAt(0).toUpperCase() + f.slice(1)} style={{ flex: 1 }}>
            <input
              type="number"
              value={doc[statsKey][f]}
              onChange={(e) => patchStats(statsKey, f, e.target.value)}
            />
          </FormField>
        ))}
      </FormRow>
    )
  }

  return (
    <div style={styles.container}>
      <div style={styles.toolbar}>
        <span style={styles.filename}>{tab.label}{tab.dirty ? ' ●' : ''}</span>
        <div style={{ flex: 1 }} />
        {error && <span style={styles.error}>{error}</span>}
        <button className="primary" onClick={handleSave} disabled={saving || !tab.dirty}>
          {saving ? 'Saving…' : 'Save'}
        </button>
      </div>

      <div style={styles.form}>
        <FormSection title="Identity">
          <FormRow>
            <FormField label="Name" style={{ flex: 2 }}>
              <input value={doc.name} onChange={(e) => update({ name: e.target.value })} />
            </FormField>
            <FormField label="Display Sprite" style={{ flex: 2 }}>
              <input value={doc.displaySpriteName} onChange={(e) => update({ displaySpriteName: e.target.value })} />
            </FormField>
            <FormField label="Behavior Key" style={{ flex: 2 }}>
              <input value={doc.behaviorKey} onChange={(e) => update({ behaviorKey: e.target.value })} />
            </FormField>
          </FormRow>
        </FormSection>

        <FormSection title="Timing">
          <FormRow>
            {(['castTime', 'activeTime', 'cooldownTime'] as const).map((field) => (
              <FormField key={field} label={field.replace(/([A-Z])/g, ' $1').trim()} style={{ flex: 1 }}>
                <input
                  type="number"
                  value={doc[field]}
                  onChange={(e) => update({ [field]: parseInt(e.target.value, 10) || 0 })}
                />
              </FormField>
            ))}
            <FormField label="Health Cost" style={{ flex: 1 }}>
              <input type="number" value={doc.healthCost} onChange={(e) => update({ healthCost: parseInt(e.target.value, 10) || 0 })} />
            </FormField>
            <FormField label="Mana Cost" style={{ flex: 1 }}>
              <input type="number" value={doc.manaCost} onChange={(e) => update({ manaCost: parseInt(e.target.value, 10) || 0 })} />
            </FormField>
          </FormRow>
        </FormSection>

        <FormSection title="Animations">
          <FormRow>
            <FormField label="Caster Animation Path" style={{ flex: 1 }}>
              <input value={doc.casterAnimationPath} onChange={(e) => update({ casterAnimationPath: e.target.value })} />
            </FormField>
            <FormField label="Target Animation Path" style={{ flex: 1 }}>
              <input value={doc.targetAnimationPath} onChange={(e) => update({ targetAnimationPath: e.target.value })} />
            </FormField>
          </FormRow>
        </FormSection>

        <FormSection title="Stat Modifiers">
          <StatGroup statsKey="statModifiers" />
        </FormSection>

        <FormSection title="Stat Requirements">
          <StatGroup statsKey="statRequirements" />
        </FormSection>
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  container: { flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' },
  toolbar: {
    display: 'flex',
    alignItems: 'center',
    gap: 10,
    padding: '6px 14px',
    borderBottom: '1px solid var(--border)',
    background: 'var(--bg-panel)',
    flexShrink: 0,
  },
  filename: { fontFamily: 'var(--font-mono)', fontSize: 12, color: 'var(--text-dim)' },
  error: { color: 'var(--error)', fontSize: 12 },
  form: {
    flex: 1,
    overflowY: 'auto',
    padding: 16,
    display: 'flex',
    flexDirection: 'column',
    gap: 14,
  },
}
