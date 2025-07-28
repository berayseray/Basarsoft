import React, { useState, useEffect } from 'react';

interface FeatureModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSave: (name: string) => void;
}

const FeatureModal: React.FC<FeatureModalProps> = ({ isOpen, onClose, onSave }) => {
    const [name, setName] = useState('');

    useEffect(() => {
        if (isOpen) {
            setName('');
        }
    }, [isOpen]);

    if (!isOpen) {
        return null;
    }

    const handleSave = () => {
        if (name.trim() === '') {
            alert('Lütfen bir isim giriniz.');
            return;
        }
        onSave(name);
    };

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <h2>Yeni Nesne Detayları</h2>
                <label htmlFor="modal-name-input">Nesne Adı:</label>
                <input
                    id="modal-name-input"
                    type="text"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    placeholder="Örn: Anıtkabir"
                    autoFocus
                />
                <div className="modal-actions">
                    <button onClick={onClose} className="button-cancel">İptal</button>
                    <button onClick={handleSave} className="button-save">Kaydet</button>
                </div>
            </div>
        </div>
    );
};

export default FeatureModal;