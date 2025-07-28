import React, { useState, useEffect, useRef } from 'react';
import axios from 'axios';
import MapComponent, { MapComponentRef } from './MapComponent';
import FeatureModal from './FeatureModal';
import './App.css';

const API_URL = 'https://localhost:44318/api/features';

interface Feature {
    id: number;
    name: string;
    locationWkt: string;
    geometryType: string;
}

interface ApiResponse<T> {
    data: T;
    message: string;
    isSuccess: boolean;
}

interface CreateFeatureDto {
    name: string;
    locationWkt: string;
}

function App() {
    const [allFeatures, setAllFeatures] = useState<Feature[]>([]);
    const [displayedFeatures, setDisplayedFeatures] = useState<Feature[]>([]);
    const [areFeaturesVisible, setAreFeaturesVisible] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [pendingWkt, setPendingWkt] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const mapComponentRef = useRef<MapComponentRef>(null);
    const [editingFeature, setEditingFeature] = useState<Feature | null>(null);

    const fetchFeatures = async () => {
        try {
            setError(null);
            const response = await axios.get<ApiResponse<Feature[]>>(API_URL);
            const data = response.data.data || [];
            setAllFeatures(data);
            if (areFeaturesVisible) {
                setDisplayedFeatures(data);
            }
        } catch (err) {
            setError('Veriler çekilirken bir hata oluştu.');
            console.error(err);
        }
    };

    useEffect(() => {
        fetchFeatures();
    }, []);

    const toggleFeatureVisibility = () => {
        setAreFeaturesVisible(prev => !prev);
    };

    useEffect(() => {
        if (editingFeature) return;
        setDisplayedFeatures(areFeaturesVisible ? allFeatures : []);
    }, [areFeaturesVisible, allFeatures, editingFeature]);

    const handleStartEdit = (featureToEdit: Feature) => {
        setEditingFeature(featureToEdit);
        setDisplayedFeatures([featureToEdit]);
    };

    const handleFeatureModified = (modifiedWkt: string) => {
        if (editingFeature) {
            setEditingFeature({ ...editingFeature, locationWkt: modifiedWkt });
        }
    };

    const handleSaveChanges = async () => {
        if (!editingFeature) return;
        const updateDto: CreateFeatureDto = { name: editingFeature.name, locationWkt: editingFeature.locationWkt };
        try {
            await axios.put(`${API_URL}/${editingFeature.id}`, updateDto);
            setEditingFeature(null);
            await fetchFeatures();
        } catch (err) {
            setError('Nesne güncellenirken bir hata oluştu.');
            console.error(err);
        }
    };

    const handleCancelEdit = () => {
        setEditingFeature(null);
        setDisplayedFeatures(areFeaturesVisible ? allFeatures : []);
    };

    const handleFeatureDrawn = (drawnWkt: string) => {
        setPendingWkt(drawnWkt);
        setIsModalOpen(true);
    };

    const handleSaveFeature = async (name: string) => {
        if (!pendingWkt) return;
        const newFeature = { name: name, locationWkt: pendingWkt };
        try {
            await axios.post(API_URL, newFeature);
            await fetchFeatures();
            setIsModalOpen(false);
            setPendingWkt(null);
            setError(null);
        } catch (err: any) {
            if (err.response?.data?.errors) {
                const errorMessages = Object.values(err.response.data.errors).flat().join('\n');
                alert(`Kayıt başarısız: ${errorMessages}`);
            } else {
                alert('Nesne eklenirken bir hata oluştu.');
            }
        }
    };

    const handleCloseModal = () => {
        setIsModalOpen(false);
        setPendingWkt(null);
        mapComponentRef.current?.clearLastDrawing();
    };

    const handleDelete = async (id: number) => {
        if (window.confirm("Bu nesneyi silmek istediğinize emin misiniz?")) {
            try {
                await axios.delete(`${API_URL}/${id}`);
                await fetchFeatures();
            } catch (err) {
                setError('Nesne silinirken bir hata oluştu.');
                console.error(err);
            }
        }
    }

    return (
        <div className="App">
            <header className="app-header">
                <h1>Türkiye Mekansal Veri Kayıt Uygulaması</h1>
            </header>

            <main className="main-content">
                <MapComponent
                    ref={mapComponentRef}
                    onFeatureDrawn={handleFeatureDrawn}
                    featuresToDisplay={displayedFeatures}
                    editingFeature={editingFeature}
                    onFeatureModified={handleFeatureModified}
                />

                {editingFeature && (
                    <div className="edit-panel">
                        <p><strong>{editingFeature.name}</strong> nesnesini düzenliyorsunuz. Haritada sürükleyerek yerini değiştirin.</p>
                        <div className="edit-panel-buttons">
                            <button className="button-save" onClick={handleSaveChanges}>Değişiklikleri Kaydet</button>
                            <button className="button-cancel" onClick={handleCancelEdit}>İptal</button>
                        </div>
                    </div>
                )}

                <FeatureModal
                    isOpen={isModalOpen}
                    onClose={handleCloseModal}
                    onSave={handleSaveFeature}
                />

                {error && <p className="error">{error}</p>}

                <div className="list-container">
                    <div className="list-header">
                        <h2>Kaydedilmiş Nesneler</h2>
                        <div className="header-buttons">
                            <button className="toggle-button" onClick={toggleFeatureVisibility} disabled={!!editingFeature}>
                                {areFeaturesVisible ? 'Nesneleri Gizle' : 'Nesneleri Göster'}
                            </button>
                            <button className="load-button" onClick={fetchFeatures} disabled={!!editingFeature}>
                                Verileri Yenile
                            </button>
                        </div>
                    </div>
                    {allFeatures.length > 0 ? (
                        <ul>
                            {allFeatures.map((feature) => (
                                <li key={feature.id} className={editingFeature?.id === feature.id ? 'editing' : ''}>
                                    <span>
                                        <strong>{feature.name}</strong> ({feature.geometryType})
                                    </span>
                                    <div className="item-buttons">
                                        <button className="edit-button" onClick={() => handleStartEdit(feature)} disabled={!!editingFeature}>Düzenle</button>
                                        <button className="delete-button" onClick={() => handleDelete(feature.id)} disabled={!!editingFeature}>Sil</button>
                                    </div>
                                </li>
                            ))}
                        </ul>
                    ) : (
                        <p>Henüz kaydedilmiş bir nesne yok.</p>
                    )}
                </div>
            </main>
        </div>
    );
}

export default App;